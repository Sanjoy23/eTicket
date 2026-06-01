using Booking.API.Application.Queries;
using Booking.API.Dtos;
using Booking.Domain.Repositories;
using MediatR;

namespace Booking.API.Application.Handlers
{
    public class BookingByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<BookingByIdQuery, BookingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<BookingDto> Handle(BookingByIdQuery request, CancellationToken cancellationToken)
        {
            var bookingEntity = await _unitOfWork.Bookings.GetById(request.BookingId)
                ?? throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");

            return new BookingDto { 
                BookingId = bookingEntity.BookingId,
                EventId = bookingEntity.EventId,
                UserId = bookingEntity.UserId,
                Status = bookingEntity.Status,
                BookingSeats = bookingEntity.BookingSeats,
                TotalAmount = bookingEntity.TotalAmount,
                CreatedAt = bookingEntity.CreatedAt
            };
        }
    }
}
