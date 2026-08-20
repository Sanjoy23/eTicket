using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class ReceiptRepository(BookingDbContext dbContext) : GenericRepository<Receipt>(dbContext), IReceiptRepository
    {
        private readonly BookingDbContext _dbContext = dbContext;

        public async Task<Receipt?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Receipts
                .FirstOrDefaultAsync(receipt => receipt.Id == id, cancellationToken);
        }
    }
}
