using MediatR;

namespace Event.API.Application.Sessions.Commands
{
    public class ConfirmSeatsCommand : IRequest
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid BookingId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
    }
}
