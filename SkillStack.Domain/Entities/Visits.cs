using System.ComponentModel.DataAnnotations;

namespace SkillStack.Domain.Entities
{
    public class Visit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VisitId { get; set; }

        [MaxLength(100)] public string Path { get; set; } = "";
        [MaxLength(20)] public string Device { get; set; } = "";
        [MaxLength(100)] public string? Referrer { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
