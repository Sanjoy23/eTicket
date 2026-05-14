using Event.API.Application.Venues.Queries;
using Event.API.Models;
using Event.Domain.Repositories;
using MediatR;
using System.Linq;

namespace Event.API.Application.Venues.Handlers
{
    public class VenueGetQueryHandler : IRequestHandler<GetVenueQeury, IEnumerable<VenueDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VenueGetQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VenueDto>> Handle(GetVenueQeury request, CancellationToken cancellationToken)
        {
            var venues = await _unitOfWork.Venues.GetAll();
            return venues.Select(v => new VenueDto
            {
                VenueId = v.VenueId,
                VenueName = v.VenueName,
                Description = v.Description,
                Capacity = v.Capacity,
                Address = v.Address,
                City = v.City,
                Country = v.Country,
                CreatedAt = v.CreatedAt
            });
        }
    }
}
