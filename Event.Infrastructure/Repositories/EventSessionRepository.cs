using Event.Domain.Entities.Events;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Repositories
{
    public class EventSessionRepository : GenericRepository<EventSession>, IEventSessionRepository
    {
        public EventSessionRepository(EventDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EventSession>> GetUpcomingSessionsByVenueId(Guid venueId, DateTime now)
        {
            return await _dbSet
                .Where(es => es.VenueId == venueId && es.EndTimeUtc > now)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
