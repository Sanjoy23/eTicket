namespace Event.API.Models.DTOs
{
    public record SeatLockResultDto
    {
        public Guid EventSessionId { get; init; }
        public Guid UserId { get; init; }
        public DateTime LockedUntilUtc { get; init; }
        public IReadOnlyCollection<Guid> SeatIds { get; init; } = Array.Empty<Guid>();
    }
}
