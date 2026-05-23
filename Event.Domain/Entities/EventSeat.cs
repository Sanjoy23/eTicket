using Event.Domain.Entities.Events;
using Event.Domain.Entities.Seating;
using Event.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities
{
    public class EventSeat
    {
        [Key]
        public Guid EventSeatId { get; set; }

        [Required]
        public Guid EventId { get; set; }

        public EventEntity Event { get; set; } = null!;

        [Required]
        public Guid SeatId { get; set; }

        public Seat Seat { get; set; } = null!;

        public SeatBookingStatus Status { get; set; }

        public decimal Price { get; set; }
    }
}
