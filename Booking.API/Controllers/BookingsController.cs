using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public IActionResult Booking(Guid userId, List<Guid> SeatIds, Guid BookingId)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Cancel(Guid userId, List<Guid> SeatIds, Guid BookingId)
        {
            return Ok();
        }
        [HttpGet]
        public IActionResult Get(Guid BookingId)
        {
            return Ok();
        }
    }
}
