using Event.API.Application.Sessions.Queries;
using Event.API.Models.DTOs;
using Event.Domain.Repositories;
using Event.Infrastructure.Specifications;
using MediatR;

namespace Event.API.Application.Sessions.Handlers
{
    public class SeesionGetByIdHandler : IRequestHandler<GetSessionByIdQuery, EventSessionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SeesionGetByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EventSessionDto> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new SessionsWithVenuesAndHallsSpecification(request.SessionId);
            var session = await _unitOfWork.EventsSession.GetBySpec(spec);
            if (session == null) {
                throw new KeyNotFoundException($"Session with ID {request.SessionId} not found.");
            }
            return new EventSessionDto {
                EventSessionId = session.EventSessionId,
                EventId = session.EventId,
                VenueName = session.Venue.VenueName,
                EndTimeUtc = session.EndTimeUtc,
                StartTimeUtc = session.StartTimeUtc,
                HallName = session.Hall.Name,
                Status = session.Status,
                TotalSeats = session.TotalSeats,
                AvailableSeats = session.AvailableSeats
            };
        }
    }
}
