using MediatR;

namespace Booking.API.Application.Commands
{
    public class BookSeatsCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public IEnumerable<Guid> SeatIds { get; set; } = [];
    }
}
