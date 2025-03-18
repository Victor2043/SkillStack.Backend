using MediatR;
using SkillStack.Domain.Entities;

namespace SkillStack.Application.Commands.UserCommands
{
    public record CreateUserCommand(string UserName, string Name, string Email, string PasswordHash) : IRequest<User>;
}
