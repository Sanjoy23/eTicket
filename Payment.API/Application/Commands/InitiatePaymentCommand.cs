using MediatR;
using Payment.API.Dtos;

namespace Payment.API.Application.Commands
{
    public class InitiatePaymentCommand : IRequest<InitiatePaymentResponse>
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
    }

    public class InitiatePaymentResponse
    {
        public Guid PaymentId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
