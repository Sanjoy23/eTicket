namespace Booking.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBookingRepository Bookings {  get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
