using Event.API.Application.Events.Queries;
using Event.API.Models;
using Event.Domain.Repositories;
using MediatR;
using System.Linq;

namespace Event.API.Application.Events.Handlers
{
    public class EventsGetHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetEventsQuery, IEnumerable<EventDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _unitOfWork.Events.GetAll();
            return events.Select(e => new EventDto
            {
                EventId = e.EventId,
                EventName = e.EventName,
                Description = e.Description,
                EventType = e.Type,
                Status = e.Status,
                CreatedAt = e.CreatedAt
            });
        }
    }
}
