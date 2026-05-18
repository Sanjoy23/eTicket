using Event.Domain.Entities.Events;

namespace Event.Domain.Repositories
{
    public interface IEventSessionRepository : IGenericRepository<EventSession>
    {
        Task<IEnumerable<EventSession>> GetUpcomingSessionsByVenueId(Guid venueId, DateTime now);
        Task<IEnumerable<EventSession>> GetEventSessionByEvent(Guid eventId);
    }
}
