using System.Linq.Expressions;
using Event.Domain.Specifications;

namespace Event.Domain.Repositories
{
    public interface IGenericRepository <T> where T : class
    {
        Task<T> GetById(Guid id);
        Task<IEnumerable<T>> GetAll();
        Task Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetBySpec(ISpecification<T> spec);
        Task<IEnumerable<T>> ListAsync(ISpecification<T> spec);
    }
}
