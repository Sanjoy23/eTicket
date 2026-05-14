using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities
{
    public class EventSeat
    {
        [Key]
        public Guid EventSeatId { get; set; }

        [Required]
        public Guid EventId { get; set; }

        public EventEntity Event { get; set; }

        [Required]
        public Guid SeatId { get; set; }

        public Seat Seat { get; set; }

        public bool IsBooked { get; set; }

        public Guid? BookingId { get; set; }

        public decimal Price { get; set; }
    }
}
