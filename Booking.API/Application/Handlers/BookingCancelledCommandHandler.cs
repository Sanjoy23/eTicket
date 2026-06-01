using Booking.API.Application.Commands;
using Booking.API.Interfaces;
using Booking.Domain.Repositories;
using MediatR;

namespace Booking.API.Application.Handlers
{
    public class BookingCancelledCommandHandler(ISeatLockService seatLockService) : IRequestHandler<CancelSeatBookingCommand, Unit>
    {
        private readonly ISeatLockService _seatLockService = seatLockService;

        public async Task<Unit> Handle(CancelSeatBookingCommand request, CancellationToken cancellationToken)
        {
            await _seatLockService.ReleaseSeatsAsync(request.SessionId, request.UserId, [..request.SeatIds], cancellationToken);
            return Unit.Value;
        }
    }
}
