namespace Event.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository Events { get; }
        IVenueRepository Venues { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
