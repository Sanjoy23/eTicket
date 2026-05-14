using Event.Domain.Enums;
using MediatR;

namespace Event.API.Application.Commands
{
    public class CreateEventCommand : IRequest<Guid>
    {
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSeats { get; set; }
        public Guid VenueId { get; set; }
        public EventType EventType { get; set; }
    }
}
