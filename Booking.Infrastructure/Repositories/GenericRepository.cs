using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private EventDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(EventDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
    }
}
