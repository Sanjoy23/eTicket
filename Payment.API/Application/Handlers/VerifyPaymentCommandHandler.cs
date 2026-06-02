using MediatR;
using Microsoft.EntityFrameworkCore;
using Payment.API.Application.Commands;
using Payment.API.Dtos;
using Payment.API.Infrastructure.Data;
using Payment.API.Interfaces;

namespace Payment.API.Application.Handlers
{
    public class VerifyPaymentCommandHandler(
        IPaymentGateway paymentGateway,
        PaymentDbContext dbContext) : IRequestHandler<VerifyPaymentCommand, PaymentVerifyResponse>
    {
        private readonly IPaymentGateway _paymentGateway = paymentGateway;
        private readonly PaymentDbContext _dbContext = dbContext;

        public async Task<PaymentVerifyResponse> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(payment => payment.PaymentId == request.PaymentId, cancellationToken)
                ?? throw new KeyNotFoundException($"Payment with ID {request.PaymentId} was not found.");

            if (payment.Status == 1)
            {
                return new PaymentVerifyResponse
                {
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    IsSuccess = true,
                    Status = "Paid",
                    TransactionId = payment.TransactionId
                };
            }

            var verifyResult = await _paymentGateway.VerifyAsync(payment.TransactionId, cancellationToken);
            payment.Status = verifyResult.IsSuccess ? 1 : 2;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new PaymentVerifyResponse
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                IsSuccess = verifyResult.IsSuccess,
                Status = verifyResult.IsSuccess ? "Paid" : "Failed",
                TransactionId = payment.TransactionId
            };
        }
    }
}
