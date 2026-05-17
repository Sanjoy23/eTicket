using Event.Domain.Entities.Events;
using System;
using System.Threading.Tasks;

namespace Event.Domain.Repositories
{
    public interface IEventSeatInventoryRepository : IGenericRepository<EventSeatInventory>
    {
        Task<int> CountAvailableSeatsByVenueId(Guid venueId);
    }
}
