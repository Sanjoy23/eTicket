using Event.Domain.Enums;
using MediatR;

namespace Event.API.Application.Events.Commands
{
    public class UpdateEventCommand : IRequest<Unit>
    {
        public required Guid EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int TotalSeats { get; set; }
        public required Guid VenueId { get; set; }
        public required EventType EventType { get; set; }
    }
}
