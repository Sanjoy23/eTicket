using Event.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities.Ticketing
{
    public class Tickets
    {
        [Key]
        public Guid TicketId { get; set; }

        public Guid BookingSeatId { get; set; }

        //public BookingSeat BookingSeat { get; set; }

        public string QRCode { get; set; } = string.Empty;

        public TicketStatus Status { get; set; }

        public DateTime IssuedAt { get; set; }
            = DateTime.UtcNow;

    }
}
