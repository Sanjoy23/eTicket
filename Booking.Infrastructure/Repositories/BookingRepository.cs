using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository(BookingDbContext context) : GenericRepository<EventBooking>(context), IBookingRepository
    {
    }
}
