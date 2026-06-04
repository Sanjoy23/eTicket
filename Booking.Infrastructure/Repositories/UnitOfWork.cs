using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public class UnitOfWork(BookingDbContext context
        , IBookingRepository bookingRepository
        , IReceiptRepository receiptRepository) : IUnitOfWork
    {
        private readonly BookingDbContext _context = context;
        public IBookingRepository Bookings { get; } = bookingRepository;
        public IReceiptRepository Receipts { get; } = receiptRepository;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
