
namespace SkillStack.Domain.Entities
{
    public class ActivationToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
