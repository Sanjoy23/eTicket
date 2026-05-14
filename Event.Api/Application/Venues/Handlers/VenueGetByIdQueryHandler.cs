using Event.API.Application.Venues.Queries;
using Event.API.Models;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Venues.Handlers
{
    public class VenueGetByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, VenueDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VenueGetByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<VenueDto> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
        {
            var venue = await _unitOfWork.Venues.GetById(request.VenueId);
            if (venue == null)
            {
                throw new KeyNotFoundException($"Venue with ID {request.VenueId} not found.");
            }

            return new VenueDto
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                Description = venue.Description,
                Capacity = venue.Capacity,
                Address = venue.Address,
                City = venue.City,
                Country = venue.Country,
                CreatedAt = venue.CreatedAt
            };
        }
    }
}
