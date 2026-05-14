using Event.API.Application.Venues.Commands;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Venues.Handlers
{
    public class VenueDeletedHandler : IRequestHandler<DeleteVenueCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VenueDeletedHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteVenueCommand request, CancellationToken cancellationToken)
        {
            var venue = await _unitOfWork.Venues.GetById(request.VenueId);
            if (venue == null)
            {
                throw new KeyNotFoundException($"Venue with ID {request.VenueId} not found.");
            }

            _unitOfWork.Venues.Delete(venue);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
