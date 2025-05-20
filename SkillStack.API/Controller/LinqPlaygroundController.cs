using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkillStack.Domain.DTOs;

namespace SkillStack.API.Controllers
{
    [ApiController]
    [Route("api/linq-playground")]
    public class LinqPlaygroundController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LinqPlaygroundController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteQuery([FromBody] LinqRequest request)
        {
            try
            {
                var query = new ExecuteLinqQuery { Query = request.Query };
                var result = await _mediator.Send(query);
                return Ok(new { Value = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}