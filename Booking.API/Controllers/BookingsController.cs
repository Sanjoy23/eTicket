using Booking.API.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("book")]
        public IActionResult Booking([FromBody] BookSeatsCommand command)
        {
            _mediator.Send(command);
            return Ok();
        }

        [HttpPost("cancel")]
        public IActionResult Cancel([FromBody] CancelSeatBookingCommand command)
        {
            return Ok();
        }
        [HttpGet("{bookingId}")]
        public IActionResult Get(Guid bookingId)
        {
            return Ok();
        }
    }
}
