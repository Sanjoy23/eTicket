using System.ComponentModel.DataAnnotations;

namespace Booking.Domain.Entities
{
    public class EventBookingSeat
    {
        [Key]
        public Guid BookingSeatId { get; set; }

        public Guid BookingId { get; set; }

        public EventBooking Booking { get; set; } = null!;

        public Guid EventSeatId { get; set; }
    }
}
