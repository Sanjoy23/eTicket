namespace Event.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository Events { get; }
        IVenueRepository Venues { get; }
        IEventSessionRepository EventsSession { get; }
        IEventSeatInventoryRepository EventsSeatInventory { get; }
        ISeatLockRepository SeatLocks { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
