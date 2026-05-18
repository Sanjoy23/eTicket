using Event.Domain.Entities.Seating;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Event.Domain.Repositories
{
    public interface ISeatLockRepository : IGenericRepository<SeatLock>
    {
        Task<IEnumerable<SeatLock>> GetActiveLocks(Guid sessionId, IEnumerable<Guid> seatIds, DateTime utcNow);
        Task<IEnumerable<SeatLock>> GetActiveLocksForUser(Guid sessionId, Guid userId, IEnumerable<Guid> seatIds, DateTime utcNow);
    }
}
