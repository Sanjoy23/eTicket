using ePayment.API.Dtos;
using ePayment.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ePayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentGateway paymentGateway) : ControllerBase
    {
        private readonly IPaymentGateway _paymentGateway = paymentGateway;

        [HttpPost("InitiatePayment")]
        public async Task<IActionResult> InitiatePaymentLink(PaymentInfoDto paymentInfo)
        {
            var newPayment = new PaymentInitiateRequest
            {
                TotalAmount = paymentInfo.Amount,
                Currency = paymentInfo.Currency,
                ReceiptId = paymentInfo.Id.ToString(),
                CustomerCity = paymentInfo.City,
                CustomerCountry = paymentInfo.Country,
                CustomerEmail = paymentInfo.Email,
                CustomerPhone = paymentInfo.Phone,
                CustomerName = paymentInfo.FullName,
                CustomerAddress1 = paymentInfo.Address,
                ProductCategory = paymentInfo.ProductType,
                ProductProfile = paymentInfo.ProductProfile,  
            };
            var result = await _paymentGateway.InitiateAsync(newPayment, CancellationToken.None);
            return Ok(result);
        }
    }
}
