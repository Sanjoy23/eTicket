using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public class ReceiptRepository(BookingDbContext dbContext) : GenericRepository<Receipt>(dbContext), IReceiptRepository
    {
    }
}
