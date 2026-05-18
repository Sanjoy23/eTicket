using Event.Domain.Enums;

namespace Event.API.Models.DTOs
{
    public record EventSessionDto
    {
        public Guid EventSessionId { get; init; }
        public Guid EventId { get; init; }
        public string VenueName { get; init; }
        public string HallName { get; init; }
        public DateTime StartTimeUtc { get; init; }
        public DateTime EndTimeUtc { get; init; }
        public SessionStatus Status { get; init; }
        public int TotalSeats { get; init; }
        public int AvailableSeats { get; init; }
    }
}
