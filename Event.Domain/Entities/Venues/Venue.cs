using Event.Domain.Entities.Events;
using Event.Domain.Entities.Seating;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities.Venues
{
    public class Venue
    {
        [Key]
        public Guid VenueId { get; set; }
        [Required]
        public string VenueName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = default!;
        [Required]
        public int Capacity { get; set; }
        [Required]
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<EventEntity> Events { get; set; } = [];
        public ICollection<Seat> Seats { get; set; } = [];
    }
}
