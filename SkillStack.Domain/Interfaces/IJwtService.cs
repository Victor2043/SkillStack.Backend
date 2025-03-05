namespace SkillStack.Application.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessToken(Guid userId, string userName, string email);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string refreshToken);
}
