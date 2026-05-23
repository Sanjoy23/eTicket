using Event.API.Application.Events.Commands;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Events.Handlers
{
    public class EventUpdatedHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateEventCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _unitOfWork.Events.GetById(request.EventId) ??
                throw new KeyNotFoundException($"Event with ID {request.EventId} not found.");
            

            if (eventEntity.Status == EventStatus.Cancelled || eventEntity.Status == EventStatus.Completed)
            {
                throw new InvalidOperationException("Cannot update a cancelled or completed event.");
            }

            eventEntity.EventName = request.EventName;
            eventEntity.Description = request.Description;
            eventEntity.Type = request.EventType;

            _unitOfWork.Events.Update(eventEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
