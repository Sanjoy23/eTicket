using Event.API.Application.Events.Commands;
using Event.API.Application.Events.Queries;
using Event.API.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Event.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;


        [HttpGet]
        public async Task<Results<Ok<IEnumerable<EventDto>>, NotFound>> GetAll()
        {
            var result = await _mediator.Send(new GetEventsQuery());
            return TypedResults.Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EventDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEventByIdQuery { EventId = id });
            return result is null ? NotFound($"Event with Id: {id} not found") : Ok(result);
        }

        [HttpPost]
        public async Task<Results<Created<Guid>, BadRequest<string>>> Create([FromBody] CreateEventCommand command)
        {
            var eventId = await _mediator.Send(command);
            return eventId == Guid.Empty
                ? TypedResults.BadRequest("Event Creation failed")
                : TypedResults.Created($"api/events/{eventId}", eventId);

        }

        [HttpPut("{id:guid}")]
        public async Task<Results<NoContent, NotFound<string>>> Update(Guid id, [FromBody] UpdateEventCommand command)
        {
            command.EventId = id;
            await _mediator.Send(command);
            return TypedResults.NoContent();
        }

        [HttpPatch("{id:guid}/cancel")]
        public async Task<Results<NoContent, NotFound<string>>> Cancel(Guid id)
        {
            await _mediator.Send(new CancelEventCommand { EventId = id });
            return TypedResults.NoContent();
        }
    }
}
