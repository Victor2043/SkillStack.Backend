using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SkillStack.Application.Commands.ActivateUserCommands;
using SkillStack.Application.Interfaces;
using SkillStack.Infrastructure.Interfaces;

namespace Tests.Application.Commands.ActivateUserCommandsTests
{
   
    public class ActivateUserCommandHandlerTests
    {
        private readonly Mock<IActivationTokenRepository> _activationTokenRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly ActivateUserCommandHandler _handler;

        public ActivateUserCommandHandlerTests()
        {
            _activationTokenRepositoryMock = new Mock<IActivationTokenRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _handler = new ActivateUserCommandHandler(
                _activationTokenRepositoryMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenTokenIsInvalid()
        {
            // Arrange
            var command = new ActivateUserCommand { Token = "invalid-token" };
            _activationTokenRepositoryMock
                .Setup(repo => repo.GetUserIdByTokenAsync(command.Token))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.ActivateUserAsync(It.IsAny<Guid>()), Times.Never);
            _activationTokenRepositoryMock.Verify(repo => repo.DeleteTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenActivationFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ActivateUserCommand { Token = "valid-token" };

            _activationTokenRepositoryMock
                .Setup(repo => repo.GetUserIdByTokenAsync(command.Token))
                .ReturnsAsync(userId);

            _userRepositoryMock
                .Setup(repo => repo.ActivateUserAsync(userId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _activationTokenRepositoryMock.Verify(repo => repo.DeleteTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldActivateUserAndDeleteToken_WhenSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ActivateUserCommand { Token = "valid-token" };

            _activationTokenRepositoryMock
                .Setup(repo => repo.GetUserIdByTokenAsync(command.Token))
                .ReturnsAsync(userId);

            _userRepositoryMock
                .Setup(repo => repo.ActivateUserAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _activationTokenRepositoryMock.Verify(repo => repo.DeleteTokenAsync(command.Token), Times.Once);
        }
    }
}

