using Booking.API.Dtos;
using Booking.API.Interfaces;

namespace Booking.API.Services
{
    public class PaymentService(IHttpClientFactory httpClientFactory) : IPaymentService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<PaymentInitiateResponseDto> InitiatePaymentAsync(PaymentInitiateRequestDto request, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("PaymentService");
            var response = await client.PostAsJsonAsync("api/Payments/initiate", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Payment initiation failed {error}");
            }

            return await response.Content.ReadFromJsonAsync<PaymentInitiateResponseDto>(cancellationToken)
                ?? throw new Exception("Payment service returned an empty initiate response.");
        }

        public async Task<PaymentVerifyResponseDto> VerifyPaymentAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("PaymentService");
            var response = await client.PostAsync($"api/Payments/{paymentId}/verify", null, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Payment verification failed {error}");
            }

            return await response.Content.ReadFromJsonAsync<PaymentVerifyResponseDto>(cancellationToken)
                ?? throw new Exception("Payment service returned an empty verify response.");
        }
    }
}
