using Event.Domain.Repositories;
using Event.Infrastructure.Data;

namespace Event.Infrastructure.Repositories
{
    public class UnitOfWork(
        EventDbContext context,
        IEventRepository eventRepository,
        IVenueRepository venueRepository,
        IEventSessionRepository eventSessionRepository,
        IEventSeatInventoryRepository eventSeatInventoryRepository,
        ISeatLockRepository seatLockRepository) : IUnitOfWork
    {
        private readonly EventDbContext _context = context;

        public IEventRepository Events { get; } = eventRepository;
        public IVenueRepository Venues { get; } = venueRepository;

        public IEventSessionRepository EventsSession { get; } = eventSessionRepository;
        public IEventSeatInventoryRepository EventSeatInventories { get; } = eventSeatInventoryRepository;
        public ISeatLockRepository SeatLocks { get; } = seatLockRepository;


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing) {
            if (disposing) { 
            _context.Dispose();
            }
        }
    }
}
