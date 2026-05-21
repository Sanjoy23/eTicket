using Event.Domain.Entities.Events;

namespace Event.Domain.Repositories
{
    public interface IEventSeatInventoryRepository : IGenericRepository<EventSeatInventory>
    {
        Task<int> CountAvailableSeatsByVenueId(Guid venueId);
        Task<IEnumerable<EventSeatInventory>> GetBySessionId(Guid sessionId);
        Task<IEnumerable<EventSeatInventory>> GetBySessionIdAndSeatIds(Guid sessionId, IEnumerable<Guid> seatIds);
        Task ConfirmSeats(Guid sessionId, Guid bookingId, Guid userId, IEnumerable<Guid> seatIds);
        Task AddInventoriesForSessionAsync(Guid sessionId, Guid hallId, decimal defaultPrice, string currency = "BDT",
        CancellationToken cancellationToken = default);
    }
}
