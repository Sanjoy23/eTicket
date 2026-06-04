using ePayment.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ePayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentGateway paymentGateway) : ControllerBase
    {
        private readonly IPaymentGateway _paymentGateway = paymentGateway;

        //[HttpPost("InitiatePayment")]
        //public Task<IActionResult> InitiatePaymentLink(string Id)
        //{
        //    return 
        //}
    }
}
