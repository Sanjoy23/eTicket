using Event.Domain.Entities.Seating;
using Event.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities.Events
{
    public class EventSeatInventory
    {
        public Guid Id { get; set; }

        public Guid EventSessionId { get; set; }

        public EventSession EventSession { get; set; } = default!;

        public Guid SeatId { get; set; }

        public Seat Seat { get; set; } = default!;

        public SeatInventoryStatus Status { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; } = "BDT";

        public Guid? BookingId { get; set; }

        public DateTime? SoldAtUtc { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
    }
}
