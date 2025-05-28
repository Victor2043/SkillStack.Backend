using Moq;
using SkillStack.Application.Commands.Login;
using SkillStack.Application.Commands.RefreshToken;
using SkillStack.Application.Interfaces;
using SkillStack.Core.Interfaces;
using SkillStack.Domain.Entities;

namespace Tests.Application.Commands.LoginCommandTests
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authServiceMock = new Mock<IAuthService>();
            _handler = new LoginCommandHandler(_userRepositoryMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var command = new LoginCommand("nonexistent@example.com", "password");

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(command.Email))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var command = new LoginCommand("user@example.com", "wrongpassword");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Name = "Test User",
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                IsActive = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(command.Email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnRefreshTokenResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var command = new LoginCommand("user@example.com", "password");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Name = "Test User",
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                IsActive = true
            };

            var tokenResponse = new RefreshTokenResponse
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _authServiceMock
                .Setup(a => a.GenerateTokens(user))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(tokenResponse.AccessToken, result.AccessToken);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }
    }
}