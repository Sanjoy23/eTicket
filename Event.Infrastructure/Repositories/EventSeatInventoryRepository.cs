using Event.Domain.Entities.Events;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Repositories
{
    public class EventSeatInventoryRepository : GenericRepository<EventSeatInventory>, IEventSeatInventoryRepository
    {
        public EventSeatInventoryRepository(EventDbContext context) : base(context)
        {
        }

        public async Task<int> CountAvailableSeatsByVenueId(Guid venueId)
        {
            return await _dbSet
                .Where(inv => inv.EventSession.VenueId == venueId && inv.Status == SeatInventoryStatus.Available)
                .AsNoTracking()
                .CountAsync();
        }
    }
}
