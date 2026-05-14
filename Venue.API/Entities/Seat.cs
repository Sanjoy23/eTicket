using System.ComponentModel.DataAnnotations;

namespace Venue.Entities
{
    public class Seat
    {
        [Key]
        public Guid SeatId { get; set; }

        [Required]
        public Guid VenueId { get; set; }

        public Venue Venue { get; set; }

        [Required]
        public string SeatNumber { get; set; } = string.Empty;

        [Required]
        public string Row { get; set; } = string.Empty;

        [Required]
        public int ColumnNumber { get; set; }

        [Required]
        public SeatType SeatType { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
