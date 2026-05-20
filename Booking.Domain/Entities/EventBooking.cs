using Booking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Booking.Domain.Entities
{
    public class EventBooking
    {
        [Key]
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public decimal TotalAmount { get; set; }

        public BookingStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public ICollection<EventBookingSeat> BookingSeats { get; set; }
            = new List<EventBookingSeat>();
    }
}
