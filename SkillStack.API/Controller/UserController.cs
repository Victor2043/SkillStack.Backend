using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillStack.Application.Commands.UserCommands;
using SkillStack.Application.Commands.ActivateUserCommands;
using SkillStack.Application.Commands.RefreshToken;

namespace SkillStack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand command)
        {
            if (command == null)
                return BadRequest("Invalid user data.");

            var user = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        [HttpPost("activate")]
        public async Task<IActionResult> Activate([FromBody] ActivateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return result ? Ok("User activated successfully.") : BadRequest("Invalid or expired token.");
        }        
    }
}
