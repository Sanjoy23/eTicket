using Event.Domain.Entities.Venues;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Repositories
{
    public class VenueRepository : GenericRepository<Venue>,IVenueRepository
    {
        public VenueRepository(EventDbContext context): base(context)
        {
            
        }

        
    }
}
