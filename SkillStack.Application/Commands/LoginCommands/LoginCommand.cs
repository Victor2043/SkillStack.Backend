using MediatR;
using SkillStack.Application.Commands.RefreshToken;

namespace SkillStack.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<RefreshTokenResponse>;
