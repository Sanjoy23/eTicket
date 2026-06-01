using Booking.Domain.Entities;
using Booking.Domain.Enums;

namespace Booking.API.Dtos
{
    public class BookingDto
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<EventBookingSeat> BookingSeats { get; set; } = [];
    }
}
