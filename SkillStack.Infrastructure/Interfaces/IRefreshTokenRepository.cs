using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillStack.Infrastructure.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task StoreTokenAsync(Guid userId, string refreshToken, DateTime expiration);
        Task<string?> GetTokenByUserIdAsync(Guid userId);
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
}
