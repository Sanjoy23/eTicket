using Booking.API.Application.Commands;
using Booking.API.Interfaces;
using MediatR;

namespace Booking.API.Application.Handlers
{
    public class BookSeatsCommandHandler(ISeatLockService seatLockService) : IRequestHandler<BookSeatsCommand, Guid>
    {
        private readonly ISeatLockService _seatLockService = seatLockService;
        public async Task<Guid> Handle(BookSeatsCommand request, CancellationToken cancellationToken)
        {
            await _seatLockService.LockSeatsAsync(request.UserId, request.SessionId, [..request.SeatIds], cancellationToken);

            var bookingId = Guid.NewGuid();
            return bookingId;
        }
    }
}
