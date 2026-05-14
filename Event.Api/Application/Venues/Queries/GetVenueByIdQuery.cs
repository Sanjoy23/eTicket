using Event.API.Models;
using MediatR;

namespace Event.API.Application.Venues.Queries
{
    public class GetVenueByIdQuery : IRequest<VenueDto>
    {
        public Guid VenueId { get; set; }
    }
}
