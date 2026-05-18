using Event.Domain.Entities.Seating;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Event.Infrastructure.Repositories
{
    public class SeatLockRepository : GenericRepository<SeatLock>, ISeatLockRepository
    {
        public SeatLockRepository(EventDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SeatLock>> GetActiveLocks(Guid sessionId, IEnumerable<Guid> seatIds, DateTime utcNow)
        {
            return await _dbSet
                .Where(lockRecord => lockRecord.EventSessionId == sessionId)
                .Where(lockRecord => seatIds.Contains(lockRecord.SeatId))
                .Where(lockRecord => lockRecord.LockedUntilUtc > utcNow)
                .ToListAsync();
        }

        public async Task<IEnumerable<SeatLock>> GetActiveLocksForUser(Guid sessionId, Guid userId, IEnumerable<Guid> seatIds, DateTime utcNow)
        {
            return await _dbSet
                .Where(lockRecord => lockRecord.EventSessionId == sessionId)
                .Where(lockRecord => lockRecord.UserId == userId)
                .Where(lockRecord => seatIds.Contains(lockRecord.SeatId))
                .Where(lockRecord => lockRecord.LockedUntilUtc > utcNow)
                .ToListAsync();
        }
    }
}
