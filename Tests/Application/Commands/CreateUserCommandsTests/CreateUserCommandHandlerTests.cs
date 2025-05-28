using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Moq;
using SkillStack.Application.Commands.UserCommands;
using SkillStack.Application.UserCommands.CreateUser;
using SkillStack.Core.Interfaces;
using SkillStack.Infrastructure.Persistence;

namespace Tests.Application.Commands.CreateUserCommandsTests
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AppDbContext _dbContext;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _jwtServiceMock = new Mock<IJwtService>();
            _configurationMock = new Mock<IConfiguration>();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                 .UseInMemoryDatabase("TestDb")
                 .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                 .Options;


            _dbContext = new AppDbContext(options);

            _configurationMock.Setup(c => c["Urls:FrontendBaseUrl"])
                .Returns("https://skillstack.app");

            _handler = new CreateUserCommandHandler(
                _dbContext,
                _emailServiceMock.Object,
                _jwtServiceMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateUserAndSendEmail()
        {
            // Arrange
            var command = new CreateUserCommand(
                UserName: "testuser",
                Name: "Test User",
                Email: "test@example.com",
                PasswordHash: "password123"
            );

            _jwtServiceMock
                .Setup(j => j.GenerateActivationToken(It.IsAny<Guid>()))
                .ReturnsAsync("activation-token");

            _emailServiceMock
                .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.UserName, result.UserName);
            Assert.Equal(command.Email, result.Email);
            Assert.False(result.IsActive);

            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
            Assert.NotNull(userInDb);

            _emailServiceMock.Verify(e => e.SendEmailAsync(
                command.Email,
                "Account Activation - SkillStack",
                It.Is<string>(s => s.Contains("activate"))
            ), Times.Once);
        }
    }
}