using Event.Domain.Entities.Seating;

namespace Event.Domain.Entities.Venues
{
    public class Hall
    {
        public Guid Id { get; set; }

        public Guid VenueId { get; set; }

        public Venue Venue { get; set; } = default!;

        public string Name { get; set; } = default!;

        public int Capacity { get; set; }

        public int TotalRows { get; set; }

        public int TotalColumns { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Seat> Seats { get; set; } = [];
    }
}
