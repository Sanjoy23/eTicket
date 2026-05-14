using Event.API.Application.Venues.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Event.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public ValuesController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        public async Task<IActionResult> Venues([FromForm] CreateVenueCommand command) {
            var result = await _mediatR.Send(command);
            return Ok();
        }
    }
}
