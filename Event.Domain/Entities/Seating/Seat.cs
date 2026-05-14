using Event.Domain.Entities.Venues;
using Event.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities.Seating
{
    public class Seat
    {
        [Key]
        public Guid SeatId { get; set; }

        public Guid HallId { get; set; }

        [Required]
        public Guid VenueId { get; set; }

        public Venue Venue { get; set; }

        public Hall Hall { get; set; } = default!;

        public string RowLabel { get; set; } = default!;

        public int SeatNumber { get; set; }

        public string SeatCode { get; set; } = default!;
        // A-1, A-2, B-1

        public SeatType SeatType { get; set; }

        public decimal XPosition { get; set; }

        public decimal YPosition { get; set; }

        public bool IsAccessible { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
