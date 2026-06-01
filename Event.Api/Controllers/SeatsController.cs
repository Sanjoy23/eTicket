using Event.API.Application.Sessions.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Event.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;


        [HttpPost("sessions/{sessionId:guid}/seats/lock")]
        public async Task<IActionResult> LockSeats(Guid sessionId, [FromBody] LockSessionSeatsCommand command)
        {
            command.SessionId = sessionId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("Seats/sessions/{sessionId}/seats/confirm")]
        public async Task<IActionResult> ConfirmSeats(Guid sessionId, ConfirmSeatsCommand command ) {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("sessions/{sessionId:guid}/seats/release")]
        public async Task<Results<NoContent, NotFound<string>>> ReleaseSeats(Guid sessionId, [FromBody] ReleaseSessionSeatsCommand command)
        {
            command.SessionId = sessionId;
            await _mediator.Send(command);
            return TypedResults.NoContent();
        }
    }
}
