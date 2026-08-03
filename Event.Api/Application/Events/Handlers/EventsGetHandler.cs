using Event.API.Application.Events.Queries;
using Event.API.Models;
using Event.API.Services;
using Event.Domain.Repositories;
using MediatR;
using System.Linq;

namespace Event.API.Application.Events.Handlers
{
    public class EventsGetHandler(IUnitOfWork unitOfWork, IRedisCacheService cache) : IRequestHandler<GetEventsQuery, IEnumerable<EventDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisCacheService _cache = cache;

        public async Task<IEnumerable<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var value = await _cache.GetAsync<IEnumerable<EventDto>>("events");
            if (value != null) {
                return value;
            }
            var events = await _unitOfWork.Events.GetAll();
            await _cache.SetAsync("events", events, TimeSpan.FromMinutes(1));
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
