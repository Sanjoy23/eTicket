using Event.API.Application.Events.Queries;
using Event.API.Models;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Events.Handlers
{
    public class EventGetByIdHandler : IRequestHandler<GetEventByIdQuery, EventDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventGetByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var eventEntity = await _unitOfWork.Events.GetById(request.EventId);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException($"Event with ID {request.EventId} not found.");
            }

            return new EventDto
            {
                EventId = eventEntity.EventId,
                EventName = eventEntity.EventName,
                Description = eventEntity.Description,
                EventType = eventEntity.Type,
                Status = eventEntity.Status,
                CreatedAt = eventEntity.CreatedAt
            };
        }
    }
}
