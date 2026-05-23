using Event.Domain.Enums;
using MediatR;

namespace Event.API.Application.Events.Commands
{
    public class CreateEventCommand : IRequest<Guid>
    {
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSeats { get; set; }
        public Guid VenueId { get; set; }
        public EventType EventType { get; set; }
    }
}
