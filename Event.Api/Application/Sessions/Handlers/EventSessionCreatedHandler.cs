using Event.API.Application.Sessions.Commands;
using Event.Domain.Entities.Events;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Sessions.Handlers
{
    public class EventSessionCreatedHandler : IRequestHandler<CreateEventSessionCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventSessionCreatedHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateEventSessionCommand request, CancellationToken cancellationToken)
        {
            var session = new EventSession
            {
                EventSessionId = Guid.NewGuid(),
                EventId = request.EventId,
                HallId = request.HallId,
                VenueId = request.VenueId,
                StartTimeUtc = request.StartTimeUtc,
                EndTimeUtc = request.EndTimeUtc,
                TotalSeats = request.TotalSeats
            };
            await _unitOfWork.EventsSession.Add(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return session.EventSessionId;
        }
    }
}
