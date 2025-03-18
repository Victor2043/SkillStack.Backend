using MediatR;
using SkillStack.Application.Interfaces;
using SkillStack.Application.Commands.RefreshToken;
using SkillStack.Core.Interfaces;

namespace SkillStack.Application.Handlers
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public RefreshTokenHandler(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RenewTokens(request.RefreshToken);
        }
    }
}
