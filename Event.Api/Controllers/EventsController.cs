using Event.API.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Event.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private IMediator _mediatR;

        public EventsController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        public async Task<IActionResult> Events([FromForm] CreateEventCommand command)
        {
            var result = await _mediatR.Send(command);
            return Ok();
        }
    }
}
