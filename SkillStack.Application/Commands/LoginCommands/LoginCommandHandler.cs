using MediatR;
using SkillStack.Application.Commands.RefreshToken;
using SkillStack.Application.Interfaces;

namespace SkillStack.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, RefreshTokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public LoginCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<RefreshTokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid Credentials");
        }

        return await _authService.GenerateTokens(user);
    }
}
