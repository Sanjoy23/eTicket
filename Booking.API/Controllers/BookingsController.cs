using Booking.API.Application.Commands;
using Booking.API.Application.Queries;
using Booking.API.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("book")]
        public async Task<IActionResult> Booking([FromBody] BookSeatsCommand command)
        {
            var result = await _mediator.Send(command);
            return result == Guid.Empty ? BadRequest("Booking Failed") : Ok(result);
        }

        [HttpPost("cancel")]
        public async Task<Results<NoContent, NotFound<string>>> Cancel([FromBody] CancelSeatBookingCommand command)
        {
            await _mediator.Send(command);
            return TypedResults.NoContent();
        }
        [HttpGet("{bookingId}")]
        public async Task<Results<Ok<BookingDto>, NotFound>> Get(Guid bookingId)
        {
            var result = await _mediator.Send(new BookingByIdQuery { BookingId = bookingId });
            return result is null 
                ? TypedResults.NotFound()
                : TypedResults.Ok(result);
        }
    }
}
