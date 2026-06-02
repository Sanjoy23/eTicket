namespace Booking.Domain.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetById(Guid id);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
    }
}
