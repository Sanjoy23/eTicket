using Event.Domain.Entities.Events;
using Event.Domain.Entities.Seating;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Repositories
{
    public class EventSeatInventoryRepository : GenericRepository<EventSeatInventory>, IEventSeatInventoryRepository
    {
        private readonly EventDbContext _context;
        public EventSeatInventoryRepository(EventDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task ConfirmSeats(Guid sessionId, Guid bookingId, Guid userId, IEnumerable<Guid> seatIds)
        {
            var seats = await _context.EventSeatInventories
                .Where(x => 
                        x.EventSessionId == sessionId && 
                        seatIds.Contains(x.SeatId))
                .ToListAsync();

            foreach(var seat in seats)
            {
                if (seat.Status != SeatInventoryStatus.Locked)
                    throw new Exception("Seat is not locked.");
                seat.Status = SeatInventoryStatus.Sold;
                seat.BookingId = bookingId;
                seat.SoldAtUtc = DateTime.UtcNow;

                
            }
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
