using Event.Domain.Entities.Venues;
using Event.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities.Events
{
    public class EventEntity
    {
        [Key]
        public Guid EventId { get; set; }
        [Required]
        public string EventName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public EventType Type { get; set; }
        public ICollection<EventPerformer> EventPerformers { get; set; } = new List<EventPerformer>();
        [Required]
        public EventStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<EventSeat> EventSeats { get; set; }
        = new List<EventSeat>();
    }
}
