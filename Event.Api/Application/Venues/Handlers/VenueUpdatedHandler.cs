using Event.API.Application.Venues.Commands;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Venues.Handlers
{
    public class VenueUpdatedHandler : IRequestHandler<UpdateVenueCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VenueUpdatedHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateVenueCommand request, CancellationToken cancellationToken)
        {
            var venue = await _unitOfWork.Venues.GetById(request.VenueId);
            if (venue == null)
            {
                throw new KeyNotFoundException($"Venue with ID {request.VenueId} not found.");
            }

            venue.VenueName = request.VenueName;
            venue.Description = request.Description;
            venue.Capacity = request.Capacity;
            venue.Address = request.Address;
            venue.City = request.City;
            venue.Country = request.Country;

            _unitOfWork.Venues.Update(venue);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
