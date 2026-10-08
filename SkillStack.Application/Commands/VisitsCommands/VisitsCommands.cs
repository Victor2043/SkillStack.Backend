using MediatR;
using System.Text.Json.Serialization;

namespace SkillStack.Application.Commands.VisitsCommands
{
    public record VisitsCommand(Guid VisitId, string Path, string? Referrer) : IRequest<Unit>
    {
        [JsonIgnore]
        public string UserAgent { get; init; } = "";
    }
}
