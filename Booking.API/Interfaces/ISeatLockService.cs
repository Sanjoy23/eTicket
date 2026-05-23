namespace Booking.API.Interfaces
{
    public interface ISeatLockService
    {
        Task LockSeatsAsync(Guid userId, Guid sessionId, List<Guid> seatIds, CancellationToken cancellationToken);
    }
}
