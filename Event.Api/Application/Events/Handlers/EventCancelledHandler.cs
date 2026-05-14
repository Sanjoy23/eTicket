using Event.API.Application.Commands;
using Event.Domain.Entities;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Events.Handlers
{
    public class EventCancelledHandler : IRequestHandler<CancelEventCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventCancelledHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CancelEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _unitOfWork.Events.GetById(request.EventId);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException($"Event with ID {request.EventId} not found.");
            }

            if (eventEntity.Status == EventStatus.Cancelled)
            {
                // Already cancelled, do nothing
                return Unit.Value;
            }

            if (eventEntity.Status == EventStatus.Completed)
            {
                throw new InvalidOperationException("Cannot cancel a completed event.");
            }

            eventEntity.Status = EventStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
