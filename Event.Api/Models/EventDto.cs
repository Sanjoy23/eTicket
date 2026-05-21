using Event.Domain.Enums;

namespace Event.API.Models
{
    public class EventDto
    {
        public Guid EventId { get; set; }
        public string? EventName { get; set; }
        public string? Description { get; set; }
        public EventType EventType { get; set; }
        public EventStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
