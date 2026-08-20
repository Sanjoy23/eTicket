using Booking.Domain.Entities;

namespace Booking.Domain.Repositories
{
    public interface IReceiptRepository : IGenericRepository<Receipt>
    {
        Task<Receipt?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}
