using Event.API.Application.Sessions.Queries;
using Event.API.Models.DTOs;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Sessions.Handlers
{
    public class EventSessionGetHandler : IRequestHandler<GetEventSessionsQuery, IEnumerable<EventSessionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventSessionGetHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EventSessionDto>> Handle(GetEventSessionsQuery request, CancellationToken cancellationToken)
        {
            var EventSessions = await _unitOfWork.EventsSession.GetEventSessionByEvent(request.EventId);
            return EventSessions.Select(e => new EventSessionDto {
                EventSessionId = e.EventSessionId,
                EventId = e.EventId,
                VenueName = e.Venue.VenueName,
                EndTimeUtc = e.EndTimeUtc,
                StartTimeUtc = e.StartTimeUtc,
                HallName = e.Hall.Name,
                Status = e.Status,
                TotalSeats = e.TotalSeats,
                AvailableSeats = e.AvailableSeats
            });
        }
    }
}
