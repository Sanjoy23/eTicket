using Event.Domain.Entities.Seating;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Event.Infrastructure.Repositories
{
    public class SeatLockRepository(EventDbContext context)
        : GenericRepository<SeatLock>(context), ISeatLockRepository
    {

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

        public async Task<List<SeatLock>> GetBySessionIdAndSeatIds(Guid sessionId, IEnumerable<Guid> seatIds)
        {
            return await _dbSet.Where(x =>
                    x.EventSessionId == sessionId &&
                    seatIds.Contains(x.SeatId)).ToListAsync();
        }

        public async Task<IEnumerable<SeatLock>> GetExpiredLocks(DateTime utcNow)
        {
            return await _dbSet.Where(x => x.LockedUntilUtc <  utcNow).ToListAsync();
        }
    }
}
