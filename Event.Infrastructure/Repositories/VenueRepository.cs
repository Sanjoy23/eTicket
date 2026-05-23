using Event.Domain.Entities.Venues;
using Event.Domain.Repositories;
using Event.Infrastructure.Data;

namespace Event.Infrastructure.Repositories
{
    public class VenueRepository(EventDbContext context) : GenericRepository<Venue>(context), IVenueRepository
    {
       
    }
}
