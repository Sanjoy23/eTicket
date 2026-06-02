using MediatR;
using Payment.API.Dtos;

namespace Payment.API.Application.Commands
{
    public class VerifyPaymentCommand : IRequest<PaymentVerifyResponse>
    {
        public Guid PaymentId { get; set; }
    }
}
