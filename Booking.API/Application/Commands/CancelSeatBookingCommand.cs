using MediatR;

namespace Booking.API.Application.Commands
{
    public class CancelSeatBookingCommand : IRequest<Unit>
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<Guid> SeatIds { get; set; } = [];
    }
}
