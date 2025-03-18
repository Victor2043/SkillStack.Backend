
namespace SkillStack.Infrastructure.Interfaces
{
    public interface IActivationTokenRepository
    {
        Task StoreTokenAsync(Guid userId, string activationToken, DateTime expiration);
        Task<Guid?> GetUserIdByTokenAsync(string activationToken);
        Task<bool> DeleteTokenAsync(string activationToken);
    }
}
