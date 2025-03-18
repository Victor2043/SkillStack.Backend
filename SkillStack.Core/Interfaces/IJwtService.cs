namespace SkillStack.Core.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessToken(Guid userId, string userName, string email);
    string GenerateRefreshToken();
    Task<string> GenerateActivationToken(Guid userId);
    Task<Guid?> ValidateActivationToken(string token);
}
