using Event.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities
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
        [Required]
        public Guid VenueId { get; set; }
        public Venue Venue { get; set; }
        public ICollection<EventPerformer> EventPerformers { get; set; } = new List<EventPerformer>();
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int TotalSeats { get; set; }

        public EventStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<EventSeat> EventSeats { get; set; }
        = new List<EventSeat>();
    }
}
