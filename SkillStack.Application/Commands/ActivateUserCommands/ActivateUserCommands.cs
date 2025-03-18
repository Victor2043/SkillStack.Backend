using MediatR;

namespace SkillStack.Application.Commands.ActivateUserCommands
{
    public record ActivateUserCommand(string Token) : IRequest<bool>;
}
