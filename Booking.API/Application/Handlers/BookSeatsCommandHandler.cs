using Booking.API.Application.Commands;
using Booking.API.Dtos;
using Booking.API.Interfaces;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using MediatR;

namespace Booking.API.Application.Handlers
{
    public class BookSeatsCommandHandler(
        ISeatLockService seatLockService,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork, IReceiptService receiptService) : IRequestHandler<BookSeatsCommand, BookSeatsResponse>
    {
        private readonly ISeatLockService _seatLockService = seatLockService;
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IReceiptService _receiptService = receiptService;

        public async Task<BookSeatsResponse> Handle(BookSeatsCommand request, CancellationToken cancellationToken)
        {
            var seatIds = request.SeatIds.ToList();
            if (seatIds.Count == 0)
            {
                throw new InvalidOperationException("At least one seat is required.");
            }

            if (request.TotalAmount <= 0)
            {
                throw new InvalidOperationException("Total amount must be greater than zero.");
            }
            var receipt = new Receipt { 
                ReceiptNumber = await _receiptService.GenerateUniqueReceiptNumberAsync(cancellationToken: cancellationToken),
                EventId = request.EventId,
                UserId = request.UserId,
                PaymentAmount = request.TotalAmount,
                PaymentDate = DateTime.UtcNow,
                CurrencyId = "BDT",
                IsPaid = false
            };
            await _unitOfWork.Receipts.AddAsync(receipt, cancellationToken);

            var bookingId = Guid.NewGuid();
            await _seatLockService.LockSeatsAsync(request.UserId, request.SessionId, seatIds, cancellationToken);

            try
            {
                var booking = new EventBooking
                {
                    BookingId = bookingId,
                    UserId = request.UserId,
                    EventId = request.EventId,
                    TotalAmount = request.TotalAmount,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    BookingSeats = [..seatIds.Select(seatId => new EventBookingSeat
                    {
                        BookingSeatId = Guid.NewGuid(),
                        BookingId = bookingId,
                        EventSeatId = seatId
                    })]
                };

                await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var payment = await _paymentService.InitiatePaymentAsync(new PaymentInitiateRequestDto
                {
                    BookingId = bookingId,
                    UserId = request.UserId,
                    Amount = request.TotalAmount,
                    Currency = request.Currency
                }, cancellationToken);

                return new BookSeatsResponse
                {
                    BookingId = bookingId,
                    PaymentId = payment.PaymentId,
                    PaymentUrl = payment.PaymentUrl,
                    PaymentStatus = payment.Status
                };
            }
            catch
            {
                await _seatLockService.ReleaseSeatsAsync(request.SessionId, request.UserId, seatIds, cancellationToken);
                throw;
            }
        }
    }
}
