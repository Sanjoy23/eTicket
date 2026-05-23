using Event.Domain.Entities.Events;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Repositories
{
    public class EventSeatInventoryRepository(EventDbContext context) 
                    : GenericRepository<EventSeatInventory>(context), IEventSeatInventoryRepository
    {
        private readonly EventDbContext _context = context;

        public async Task AddInventoriesForSessionAsync(Guid sessionId, Guid hallId, decimal defaultPrice, string currency = "BDT", CancellationToken cancellationToken = default)
        {
            var seats = await _context.Seats
                .Where(x => x.HallId == hallId && x.IsActive)
                .ToListAsync(cancellationToken);

            var inventories = seats.Select(seat => new EventSeatInventory
            {
                Id = Guid.NewGuid(),
                EventSessionId = sessionId,
                SeatId = seat.SeatId,
                Status = SeatInventoryStatus.Available,
                Price = defaultPrice,
                Currency = currency

            });
            await _context.EventSeatInventories.AddRangeAsync(inventories, cancellationToken);
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
