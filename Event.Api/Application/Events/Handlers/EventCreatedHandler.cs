using Event.API.Application.Events.Commands;
using Event.Domain.Entities.Events;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Events.Handlers
{
    public class EventCreatedHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = new EventEntity
            {
                EventId = Guid.NewGuid(),
                EventName = request.EventName,
                Type = request.EventType,
                Description = request.Description,
                Status = EventStatus.Upcoming
            };

            await _unitOfWork.Events.Add(newEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return newEvent.EventId;
        }
    }
}
