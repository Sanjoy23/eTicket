using Event.API.Models;
using MediatR;

namespace Event.API.Application.Events.Queries
{
    public class GetEventByIdQuery : IRequest<EventDto>
    {
        public Guid EventId { get; set; }
    }
}
