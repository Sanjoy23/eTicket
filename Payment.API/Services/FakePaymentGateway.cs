using Payment.API.Dtos;
using Payment.API.Interfaces;

namespace Payment.API.Services
{
    public class FakePaymentGateway : IPaymentGateway
    {
        public Task<PaymentInitiateResult> InitiateAsync(PaymentInitiateRequest request, CancellationToken cancellationToken)
        {
            var transactionId = $"FAKE-{Guid.NewGuid()}";

            var result = new PaymentInitiateResult
            {
                TransactionId = transactionId,
                PaymentUrl = $"https://fake-payment.com/pay?transactionId={transactionId}"
            };

            return Task.FromResult(result);
        }

        public Task<PaymentVerifyResult> VerifyAsync(string transactionId, CancellationToken cancellationToken)
        {
            var result = new PaymentVerifyResult
            {
                IsSuccess = true,
                TransactionId = transactionId,
                RawResponse = "Fake payment verified successfully"
            };

            return Task.FromResult(result);
        }
    }
}
