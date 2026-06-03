using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ePayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IMediator mediator) : ControllerBase
    {
        
    }
}
