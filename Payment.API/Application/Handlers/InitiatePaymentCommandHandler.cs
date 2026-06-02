using MediatR;
using Payment.API.Application.Commands;
using Payment.API.Dtos;
using Payment.API.Infrastructure.Data;
using Payment.API.Interfaces;
using Payment.API.Models;

namespace Payment.API.Application.Handlers
{
    public class InitiatePaymentCommandHandler(
        IPaymentGateway paymentGateway,
        PaymentDbContext dbContext) : IRequestHandler<InitiatePaymentCommand, InitiatePaymentResponse>
    {
        private readonly IPaymentGateway _paymentGateway = paymentGateway;
        private readonly PaymentDbContext _dbContext = dbContext;

        public async Task<InitiatePaymentResponse> Handle(
        InitiatePaymentCommand request,
        CancellationToken cancellationToken)
        {
            var paymentId = Guid.NewGuid();

            var gatewayResult = await _paymentGateway.InitiateAsync(
                new PaymentInitiateRequest
                {
                    BookingId = request.BookingId,
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Currency = request.Currency
                },
                cancellationToken);

            var payment = new PaymentEntity
            {
                PaymentId = paymentId,
                BookingId = request.BookingId,
                UserId = request.UserId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = 0,
                TransactionId = gatewayResult.TransactionId,
                GatewayPaymentUrl = gatewayResult.PaymentUrl,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _dbContext.Payments.AddAsync(payment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new InitiatePaymentResponse
            {
                PaymentId = payment.PaymentId,
                PaymentUrl = gatewayResult.PaymentUrl,
                Status = "Pending"
            };
        }
    }
}
