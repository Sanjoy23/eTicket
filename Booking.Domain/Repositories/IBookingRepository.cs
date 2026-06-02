using Booking.Domain.Entities;

namespace Booking.Domain.Repositories
{
    public interface IBookingRepository: IGenericRepository<EventBooking>
    {
        Task<EventBooking?> GetByIdWithSeatsAsync(Guid bookingId, CancellationToken cancellationToken = default);
    }
}
