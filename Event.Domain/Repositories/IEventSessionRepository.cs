using Event.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Event.Domain.Repositories
{
    public interface IEventSessionRepository : IGenericRepository<EventSession>
    {
        Task<IEnumerable<EventSession>> GetUpcomingSessionsByVenueId(Guid venueId, DateTime now);
    }
}
