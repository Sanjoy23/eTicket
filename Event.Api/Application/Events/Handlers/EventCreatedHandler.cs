using Event.API.Application.Commands;
using Event.Domain.Entities;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Events.Handlers
{
    public class EventCreatedHandler : IRequestHandler<CreateEventCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventCreatedHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = new EventEntity
            {
                EventId = Guid.NewGuid(),
                EventName = request.EventName,
                Type = request.EventType,
                VenueId = request.VenueId,
                Description = request.Description,
                TotalSeats = request.TotalSeats,
                StartDate = request.StartDate,
                EndDate = request.EndDate,

            };
            await _unitOfWork.Events.Add(newEvent);
            return await _unitOfWork.SaveChangesAsync(cancellationToken);
            
        }
    }
}
