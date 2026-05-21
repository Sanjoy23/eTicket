using Event.Domain.Entities.Venues;
using Event.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities.Events
{
    public class EventSession
    {
        [Key]
        public Guid EventSessionId { get; set; }

        [Required]
        public Guid EventId { get; set; }

        public EventEntity Event { get; set; } = default!;

        [Required]
        public Guid VenueId { get; set; }

        public Venue Venue { get; set; } = default!;

        [Required]
        public Guid HallId { get; set; }

        public Hall Hall { get; set; } = default!;

        [Required]
        public DateTime StartTimeUtc { get; set; }

        [Required]
        public DateTime EndTimeUtc { get; set; }

        public SessionStatus Status { get; set; }

        public int TotalSeats { get; set; }

        public int AvailableSeats { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public ICollection<EventSeatInventory> SeatInventories { get; set; }
            = [];
    }
}
