using Event.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities
{
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public EventEntity Event { get; set; }

        public decimal TotalAmount { get; set; }

        public BookingStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public ICollection<BookingSeat> BookingSeats { get; set; }
            = new List<BookingSeat>();
    }
}
