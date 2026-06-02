using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.API.Application.Commands;

namespace Payment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] InitiatePaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{paymentId:guid}/verify")]
        public async Task<IActionResult> Verify(Guid paymentId)
        {
            var result = await _mediator.Send(new VerifyPaymentCommand { PaymentId = paymentId });
            return Ok(result);
        }
    }
}
