using MediatR;

namespace Booking.API.Application.Commands
{
    public class BookSeatsCommand : IRequest<Guid>
    {
        public Guid userId { get; set; }
        public Guid SessionId { get; set; }
        public IEnumerable<Guid> SeatIds { get; set; } = Array.Empty<Guid>();
    }
}
