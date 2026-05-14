using Event.API.Models;
using MediatR;
using System.Collections.Generic;

namespace Event.API.Application.Venues.Queries
{
    public class GetVenueQeury : IRequest<IEnumerable<VenueDto>>
    {
    }
}
