using Event.API.Application.Sessions.Commands;
using Event.API.Application.Sessions.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Event.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class EventSessionsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("events/{eventId:guid}/sessions")]
        public async Task<IActionResult> Create(Guid eventId, [FromBody] CreateEventSessionCommand command)
        {
            command.EventId = eventId;
            var sessionId = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new {eventId = command.EventId, sessionId}, sessionId);

        }

        [HttpGet("events/{eventId:guid}/sessions")]
        public async Task<IActionResult> GetByEvent(Guid eventId)
        {
            var result = await _mediator.Send(new GetEventSessionsQuery { EventId = eventId});
            return Ok(result);
        }

        [HttpGet("sessions/{sessionId:guid}")]
        public async Task<IActionResult> Get(Guid sessionId)
        {
            var result = await _mediator.Send(new GetSessionByIdQuery { SessionId = sessionId });
            return Ok(result);
        }

        [HttpGet("sessions/{sessionId:guid}/seat-map")]
        public async Task<IActionResult> GetSeatMap(Guid sessionId)
        {
            var result = await _mediator.Send(new GetSessionSeatMapQuery { SessionId = sessionId });
            return Ok(result);
        }
    }
}
