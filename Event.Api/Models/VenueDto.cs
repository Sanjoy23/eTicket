namespace Event.API.Models
{
    public record VenueDto
    {
        public Guid VenueId { get; init; }
        public string VenueName { get; init; }
        public string Description { get; init; }
        public int Capacity { get; init; }
        public string Address { get; init; }
        public string City { get; init; }
        public string Country { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
