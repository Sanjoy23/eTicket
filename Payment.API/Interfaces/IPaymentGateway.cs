using Payment.API.Dtos;

namespace Payment.API.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentInitiateResult> InitiateAsync(PaymentInitiateRequest request, CancellationToken cancellationToken);
        Task<PaymentVerifyResult> VerifyAsync(string transactionId, CancellationToken cancellationToken);
    }
}
