namespace Booking.API.Interfaces
{
    public interface ISeatLockService
    {
        Task LockSeatsAsync(Guid userId, Guid sessionId, Guid bookingId, List<Guid> seatIds, CancellationToken cancellationToken);
    }
}
