using Event.API.Application.Sessions.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Event.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventSessionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventSessionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("events/{eventId:guid}/sessions")]
        public async Task<IActionResult> Create(Guid eventId, [FromBody] CreateEventSessionCommand command)
        {
            command.EventId = eventId;
            var sessionId = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new {eventId = command.EventId, sessionId}, sessionId);

        }
    }
}
