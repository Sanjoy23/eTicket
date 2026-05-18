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

        public async Task<IEnumerable<EventSeatInventory>> GetBySessionId(Guid sessionId)
        {
            return await _dbSet
                .Include(inv => inv.Seat)
                .Where(inv => inv.EventSessionId == sessionId)
                .AsNoTracking()
                .OrderBy(inv => inv.Seat.RowLabel)
                .ThenBy(inv => inv.Seat.SeatNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventSeatInventory>> GetBySessionIdAndSeatIds(Guid sessionId, IEnumerable<Guid> seatIds)
        {
            return await _dbSet
                .Where(inv => inv.EventSessionId == sessionId)
                .Where(inv => seatIds.Contains(inv.SeatId))
                .ToListAsync();
        }
    }
}
