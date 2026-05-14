using Event.Domain.Repositories;
using Event.Infrastructure.Data;

namespace Event.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventDbContext _context;

        public IEventRepository Events { get; }
        public IVenueRepository Venues { get; }

        public UnitOfWork(
            EventDbContext context, 
            IEventRepository eventRepository, 
            IVenueRepository venueRepository)
        {
            _context = context;
            Events = eventRepository;
            Venues = venueRepository;
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
