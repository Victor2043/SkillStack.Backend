using System.Threading.Tasks;
using SkillStack.Application.Commands.RefreshToken;
using SkillStack.Domain.Entities;

namespace SkillStack.Application.Interfaces;

public interface IAuthService
{
    Task<RefreshTokenResponse> GenerateTokens(User user);
    Task<RefreshTokenResponse> RenewTokens(string refreshToken);
}
