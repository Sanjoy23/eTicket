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
            await _seatLockService.LockSeatsAsync(request.userId, request.SessionId, request.SeatIds.ToList(), cancellationToken);

            var bookingId = Guid.NewGuid();
            return bookingId;
        }
    }
}
