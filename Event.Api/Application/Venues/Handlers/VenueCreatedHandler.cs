using Event.API.Application.Venues.Commands;
using Event.Domain.Entities.Venues;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Venues.Handlers
{
    public class VenueCreatedHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateVenueCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
        {
            var newVenue = new Venue
            {
                VenueId = Guid.NewGuid(),
                VenueName = request.VenueName,
                Description = request.Description,
                Capacity = request.Capacity,
                Address = request.Address,
                City = request.City,
                Country = request.Country
            };

            await _unitOfWork.Venues.Add(newVenue);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return newVenue.VenueId;
        }
    }
}
