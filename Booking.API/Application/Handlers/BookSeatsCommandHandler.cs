using Booking.API.Application.Commands;
using Booking.API.Interfaces;
using MediatR;

namespace Booking.API.Application.Handlers
{
    public class BookSeatsCommandHandler : IRequestHandler<BookSeatsCommand, Guid>
    {
        private readonly ISeatLockService _seatLockService;

        public BookSeatsCommandHandler(ISeatLockService seatLockService)
        {
            _seatLockService = seatLockService;
        }

        public async Task<Guid> Handle(BookSeatsCommand request, CancellationToken cancellationToken)
        {
            var bookingId = Guid.NewGuid();
            await _seatLockService.LockSeatsAsync(request.userId, request.SessionId, bookingId, request.SeatIds.ToList(), cancellationToken);

            return bookingId;
        }
    }
}
