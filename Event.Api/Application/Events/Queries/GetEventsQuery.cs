using Event.API.Models;
using MediatR;
using System.Collections.Generic;

namespace Event.API.Application.Events.Queries
{
    public class GetEventsQuery : IRequest<IEnumerable<EventDto>>
    {
    }
}
