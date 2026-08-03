using Event.Domain.Enums;
using MediatR;

namespace Event.API.Application.Sessions.Commands
{
    public class CreateEventSessionCommand : IRequest<Guid>
    {
        public required Guid EventId { get; set; }
        public required Guid VenueId { get; set; }

        public required Guid HallId { get; set; }

        public required DateTime StartTimeUtc { get; set; }

        public required DateTime EndTimeUtc { get; set; }

        public required SessionStatus Status { get; set; }
        public required int TotalSeats { get; set; }

        public required int AvailableSeat { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
    }
}
