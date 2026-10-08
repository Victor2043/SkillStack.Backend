using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillStack.Application.Commands.UserCommands;
using SkillStack.Application.Commands.ActivateUserCommands;
using SkillStack.Domain.Entities;
using Microsoft.AspNetCore.RateLimiting;
using SkillStack.Application.Commands.VisitsCommands;

namespace SkillStack.API.Controllers
{
    [AllowAnonymous]
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
        
        [HttpGet("activate")]
        public async Task<IActionResult> Activate([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Activation token is required." });

            var command = new ActivateUserCommand { Token = token };
            var result = await _mediator.Send(command);

            return result
                ? Ok(new { message = "User activated successfully." })
                : BadRequest(new { message = "Invalid or expired token." });
        }
        
        [HttpPost("visits")]
        [EnableRateLimiting("visits")]
        public async Task<IActionResult> Visits([FromBody] VisitsCommand command, CancellationToken  ct)
        {
            await _mediator.Send(
                command with { UserAgent = Request.Headers.UserAgent.ToString() }, ct);

            return NoContent();
        }
    }
}
