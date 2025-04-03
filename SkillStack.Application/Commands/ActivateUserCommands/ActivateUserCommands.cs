using MediatR;

namespace SkillStack.Application.Commands.ActivateUserCommands
{
    public class ActivateUserCommand : IRequest<bool>
    {
        public string Token { get; set; }
    }
}
