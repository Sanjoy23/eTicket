namespace Booking.API.Interfaces
{
    public interface ISeatLockService
    {
        Task LockSeatsAsync(Guid userId, Guid sessionId, List<Guid> seatIds, CancellationToken cancellationToken);
        Task ReleaseSeatsAsync(Guid sessionId, Guid userId, List<Guid> seatIds, CancellationToken cancellationToken);
    }
}
