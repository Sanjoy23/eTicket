using Event.API.Application.Venues.Commands;
using Event.API.Application.Venues.Queries;
using Event.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Event.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VenuesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VenueDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetVenueQeury());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VenueDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetVenueByIdQuery { VenueId = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVenueCommand command)
        {
            var venueId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = venueId }, null);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVenueCommand command)
        {
            command.VenueId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteVenueCommand { VenueId = id });
            return NoContent();
        }
    }
}
