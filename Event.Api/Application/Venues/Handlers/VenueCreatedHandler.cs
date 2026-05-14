using Event.API.Application.Venues.Commands;
using Event.Domain.Entities;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Venues.Handlers
{
    public class VenueCreatedHandler : IRequestHandler<CreateVenueCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VenueCreatedHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
        {
            var newVenue = new Venue
            {
                VenueId = Guid.NewGuid(),
                VenueName = request.VenueName,
                Description = request.Description,
                Capacity = request.Capacity,
                Address = request.Address

            };
            await _unitOfWork.Venues.Add(newVenue);
            return await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
