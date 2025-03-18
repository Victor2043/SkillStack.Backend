using MediatR;
using SkillStack.Application.Interfaces;
using SkillStack.Infrastructure.Interfaces;


namespace SkillStack.Application.Commands.ActivateUserCommands
{
    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, bool>
    {
        private readonly IActivationTokenRepository _activationTokenRepository;
        private readonly IUserRepository _userRepository;

        public ActivateUserCommandHandler(IActivationTokenRepository activationTokenRepository, IUserRepository userRepository)
        {
            _activationTokenRepository = activationTokenRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var userId = await _activationTokenRepository.GetUserIdByTokenAsync(request.Token);
            if (userId == null)
                return false;

            var activated = await _userRepository.ActivateUserAsync(userId.Value);
            if (activated)
            {
                await _activationTokenRepository.DeleteTokenAsync(request.Token);
            }

            return activated;
        }
    }
}
