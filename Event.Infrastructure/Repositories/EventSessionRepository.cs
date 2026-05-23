using Event.Domain.Entities.Events;
using Event.Domain.Entities.Venues;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Repositories
{
    public class EventSessionRepository(EventDbContext context) 
        : GenericRepository<EventSession>(context), IEventSessionRepository
    {

        public async Task<IEnumerable<EventSession>> GetUpcomingSessionsByVenueId(Guid venueId, DateTime now)
        {
            return await _dbSet
                .Where(es => es.VenueId == venueId && es.EndTimeUtc > now)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<EventSession>> GetEventSessionByEvent(Guid eventId) { 
            return await _dbSet
                .Where(es => es.EventId == eventId)
                .Include(es => es.Venue)
                .Include(es => es.Hall).ToListAsync();
        }
    }
}
