namespace Event.Domain.Entities.Events
{
    public class EventPerformer
    {
        public Guid EventId { get; set; }
        public EventEntity Event { get; set; } = null!;

        public Guid PerformerId { get; set; }
        public Performer Performer { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
