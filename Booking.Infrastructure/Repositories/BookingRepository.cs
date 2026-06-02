using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository(BookingDbContext context) : GenericRepository<EventBooking>(context), IBookingRepository
    {
        private readonly BookingDbContext _context = context;

        public async Task<EventBooking?> GetByIdWithSeatsAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Bookings
                .Include(booking => booking.BookingSeats)
                .FirstOrDefaultAsync(booking => booking.BookingId == bookingId, cancellationToken);
        }
    }
}
