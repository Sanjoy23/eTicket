using Event.API.Application.Venues.Queries;
using Event.Domain.Repositories;
using MediatR;
using System.Linq;
using Event.API.Models.DTOs;

namespace Event.API.Application.Venues.Handlers
{
    public class VenueAvailabilityByIDQuery : IRequestHandler<GetVenueAvailabiltiyById, VenueAvailabilityDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VenueAvailabilityByIDQuery(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<VenueAvailabilityDto> Handle(GetVenueAvailabiltiyById request, CancellationToken cancellationToken)
        {
            var venue = await _unitOfWork.Venues.GetById(request.VenueId);
            if (venue == null)
                throw new KeyNotFoundException($"Venue with ID {request.VenueId} not found.");

            var now = DateTime.UtcNow;
            var sessions = (await _unitOfWork.EventsSession.GetUpcomingSessionsByVenueId(venue.VenueId, now)).ToList();

            var upcomingSessions = sessions.Count;
            var totalCapacity = sessions.Sum(s => s.TotalSeats);
            var remainingCapacity = await _unitOfWork.EventSeatInventories.CountAvailableSeatsByVenueId(venue.VenueId);

            var nextSession = sessions
                .Where(s => s.AvailableSeats > 0 && s.StartTimeUtc > now)
                .OrderBy(s => s.StartTimeUtc)
                .FirstOrDefault();

            return new VenueAvailabilityDto
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                IsDeleted = venue.IsDeleted,
                TotalCapacity = totalCapacity,
                RemainingCapacity = remainingCapacity,
                UpcomingSessions = upcomingSessions,
                NextAvailableSessionUtc = nextSession?.StartTimeUtc,
                IsAvailable = remainingCapacity > 0 && upcomingSessions > 0
            };
        }
    }
}
