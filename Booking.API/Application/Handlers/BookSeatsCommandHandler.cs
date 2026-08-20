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
            
            var bookingId = Guid.NewGuid();
            var receipt = new Receipt
            {
                ReceiptNumber = await _receiptService.GenerateUniqueReceiptNumberAsync(cancellationToken: cancellationToken),
                EventId = request.EventId,
                UserId = request.UserId,
                PaymentAmount = request.TotalAmount,
                PaymentDate = DateTime.UtcNow,
                CurrencyId = request.Currency,
                IsPaid = false
            };
            await _unitOfWork.Receipts.AddAsync(receipt, cancellationToken);

            await _seatLockService.LockSeatsAsync(request.UserId, request.SessionId, seatIds, cancellationToken);

            EventBooking? booking = null;
            try
            {
                booking = new EventBooking
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
                    Id = receipt.Id,
                    BookingId = bookingId,
                    UserId = request.UserId,
                    ReceiptNumber = receipt.ReceiptNumber,
                    Amount = request.TotalAmount,
                    Currency = request.Currency,
                    FullName = "Test User",
                    Email = "test@gmail.com",
                    Phone = "01700000000",
                    City = "Dhaka",
                    Country = "Bangladesh",
                    Address = "Mohammadpur",
                    ProductType = "Theatre",
                    ProductProfile = "General"

                }, cancellationToken);

                if (payment.Status == "Failed" || payment.Status == "Cancel" || payment.Status == "Cancelled")
                {
                    booking.Status = BookingStatus.Cancelled;
                    await _seatLockService.ReleaseSeatsAsync(request.SessionId, request.UserId, seatIds, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

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
                if (booking is not null)
                {
                    booking.Status = BookingStatus.Cancelled;
                    await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                }

                throw;
            }
        }
    }
}
