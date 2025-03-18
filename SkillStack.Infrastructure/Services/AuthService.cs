using SkillStack.Application.Interfaces;
using SkillStack.Application.Commands.RefreshToken;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Interfaces;
using SkillStack.Core.Interfaces;

namespace SkillStack.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(IJwtService jwtService, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<RefreshTokenResponse> GenerateTokens(User user)
    {
        var accessToken = await _jwtService.GenerateAccessToken(user.Id, user.UserName, user.Email);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _refreshTokenRepository.StoreTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<RefreshTokenResponse> RenewTokens(string refreshToken)
    {
        var userId = await _refreshTokenRepository.GetTokenByUserIdAsync(Guid.Parse(refreshToken));
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid Refresh Token");

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
        return await GenerateTokens(user);
    }
}
