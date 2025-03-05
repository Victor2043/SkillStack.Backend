using MediatR;

namespace SkillStack.Application.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public required string RefreshToken { get; set; }
    }
}
