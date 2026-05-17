using Event.Domain.Repositories;
using Event.Infrastructure.Data;

namespace Event.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventDbContext _context;

        public IEventRepository Events { get; }
        public IVenueRepository Venues { get; }

        public IEventSessionRepository EventsSession { get; }   
        public IEventSeatInventoryRepository EventsSeatInventory { get; }

        public UnitOfWork(
            EventDbContext context, 
            IEventRepository eventRepository, 
            IVenueRepository venueRepository, IEventSessionRepository eventSessionRepository,
            IEventSeatInventoryRepository eventSeatInventoryRepository)
        {
            _context = context;
            Events = eventRepository;
            Venues = venueRepository;
            EventsSession = eventSessionRepository;
            EventsSeatInventory = eventSeatInventoryRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) {
            if (disposing) { 
            _context.Dispose();
            }
        }
    }
}
