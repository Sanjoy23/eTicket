using Event.Domain.Entities;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Repositories
{
    public class EventRepository : GenericRepository<EventEntity>, IEventRepository
    {
        public EventRepository(EventDbContext context) : base(context)
        {
        }
    }
}
