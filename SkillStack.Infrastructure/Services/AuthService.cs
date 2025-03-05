using SkillStack.Application.Interfaces;
using SkillStack.Application.Commands.RefreshToken;
using SkillStack.Domain.Entities;

namespace SkillStack.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;

    public AuthService(IJwtService jwtService, IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
    }

    public async Task<RefreshTokenResponse> GenerateTokens(User user)
    {
        var accessToken = await _jwtService.GenerateAccessToken(user.Id, user.UserName, user.Email);
        var refreshToken = _jwtService.GenerateRefreshToken();
        JwtService.StoreRefreshToken(user.Id.ToString(), refreshToken);

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<RefreshTokenResponse> RenewTokens(string refreshToken)
    {
        var userId = JwtService.GetUserIdByRefreshToken(refreshToken);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid Refresh Token");

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        return await GenerateTokens(user);
    }
}
