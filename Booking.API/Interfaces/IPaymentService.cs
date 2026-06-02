using Booking.API.Dtos;

namespace Booking.API.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentInitiateResponseDto> InitiatePaymentAsync(PaymentInitiateRequestDto request, CancellationToken cancellationToken);
        Task<PaymentVerifyResponseDto> VerifyPaymentAsync(Guid paymentId, CancellationToken cancellationToken);
    }
}
