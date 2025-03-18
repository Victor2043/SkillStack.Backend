using Microsoft.EntityFrameworkCore;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Interfaces;
using SkillStack.Infrastructure.Persistence;

namespace SkillStack.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task StoreTokenAsync(Guid userId, string refreshToken, DateTime expiration)
        {
            var token = new RefreshToken { UserId = userId, Token = refreshToken, Expiration = expiration };
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetTokenByUserIdAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.Expiration > DateTime.UtcNow)
                .Select(t => t.Token)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (token == null) return false;
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
