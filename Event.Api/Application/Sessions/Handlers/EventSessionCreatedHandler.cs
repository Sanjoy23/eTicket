using Event.API.Application.Sessions.Commands;
using Event.Domain.Entities.Events;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Sessions.Handlers
{
    public class EventSessionCreatedHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateEventSessionCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(CreateEventSessionCommand request, CancellationToken cancellationToken)
        {
            var isOverlapping = await _unitOfWork.EventsSession
                .AnyAsync(es =>
                es.HallId == request.HallId &&
                es.Status != SessionStatus.Cancelled &&
                request.StartTimeUtc < es.EndTimeUtc &&
                request.EndTimeUtc > es.StartTimeUtc);

            if (isOverlapping) throw new Exception("Time overlapping");

            var session = new EventSession
            {
                EventSessionId = Guid.NewGuid(),
                EventId = request.EventId,
                HallId = request.HallId,
                VenueId = request.VenueId,
                StartTimeUtc = request.StartTimeUtc,
                EndTimeUtc = request.EndTimeUtc,
                Status = request.Status,
                TotalSeats = request.TotalSeats
            };
            await _unitOfWork.EventsSession.Add(session);
            await _unitOfWork.EventSeatInventories.AddInventoriesForSessionAsync(session.EventSessionId, session.HallId, 500,"BDT", cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return session.EventSessionId;
        }
    }
}
