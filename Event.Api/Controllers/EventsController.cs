using Event.API.Application.Events.Commands;
using Event.API.Application.Events.Queries;
using Event.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Event.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetEventsQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EventDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEventByIdQuery { EventId = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventCommand command)
        {
            var eventId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = eventId }, null);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventCommand command)
        {
            command.EventId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _mediator.Send(new CancelEventCommand { EventId = id });
            return NoContent();
        }
    }
}
