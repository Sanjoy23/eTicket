namespace Event.API.Models.DTOs
{
    public record VenueAvailabilityDto
    {
        public Guid VenueId { get; init; }
        public string VenueName { get; init; } = string.Empty;
        public bool IsAvailable { get; init; }
        public string Status { get; init; } = string.Empty;
        public int TotalCapacity { get; init; }
        public int RemainingCapacity { get; init; }
        public int UpcomingSessions { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? NextAvailableSessionUtc { get; init; }
    }
}
