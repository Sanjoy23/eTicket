using ePayment.API.Dtos;
using ePayment.API.Models;
using Microsoft.Extensions.Options;

namespace ePayment.API.Services
{
    public interface IPaymentGateway
    {
        Task<PaymentInitiateResult> InitiateAsync(PaymentInitiateRequest request, CancellationToken cancellationToken);
        Task<PaymentVerifyResult> VerifyAsync(string transactionId, CancellationToken cancellationToken);
    }
}
