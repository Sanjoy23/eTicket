using MediatR;

namespace Event.API.Application.Sessions.Commands
{
    public class ConfirmSeatsCommand : IRequest
    {
        public required Guid SessionId { get; set; }
        public required Guid UserId { get; set; }
        public required Guid BookingId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
    }
}
