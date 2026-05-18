using Event.API.Models;
using MediatR;

namespace Event.API.Application.Events.Queries
{
    public class GetEventsQuery : IRequest<IEnumerable<EventDto>>
    {
    }
}
