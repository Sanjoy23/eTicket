using Event.API.Models.DTOs;
using MediatR;

namespace Event.API.Application.Venues.Queries
{
    public class GetVenueAvailabiltiyById : IRequest<VenueAvailabilityDto>
    {
        public Guid VenueId { get; set; }
    }
}
