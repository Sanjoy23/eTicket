using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities.Booking
{
    public class BookingSeat
    {
        [Key]
        public Guid BookingSeatId { get; set; }

        public Guid BookingId { get; set; }

        public Booking Booking { get; set; }

        public Guid EventSeatId { get; set; }

        public EventSeat EventSeat { get; set; }
    }
}
