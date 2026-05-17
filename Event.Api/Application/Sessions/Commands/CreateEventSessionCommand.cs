using MediatR;

namespace Event.API.Application.Sessions.Commands
{
    public class CreateEventSessionCommand : IRequest<Guid>
    {
        public Guid EventId { get; set; }
        public Guid VenueId { get; set; }

        public Guid HallId { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }
        public int TotalSeats { get; set; }
    }
}
