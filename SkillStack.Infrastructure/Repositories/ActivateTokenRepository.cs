using Microsoft.EntityFrameworkCore;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Interfaces;
using SkillStack.Infrastructure.Persistence;

namespace SkillStack.Infrastructure.Repositories
{
    public class ActivationTokenRepository : IActivationTokenRepository
    {
        private readonly AppDbContext _context;

        public ActivationTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task StoreTokenAsync(Guid userId, string activationToken, DateTime expiration)
        {
            var token = new ActivationToken { UserId = userId, Token = activationToken, Expiration = expiration };
            await _context.ActivationTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<Guid?> GetUserIdByTokenAsync(string activationToken)
        {
            var token = await _context.ActivationTokens.FirstOrDefaultAsync(t => t.Token == activationToken && t.Expiration > DateTime.UtcNow);
            return token?.UserId;
        }

        public async Task<bool> DeleteTokenAsync(string activationToken)
        {
            var token = await _context.ActivationTokens.FirstOrDefaultAsync(t => t.Token == activationToken);
            if (token == null) return false;
            _context.ActivationTokens.Remove(token);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
