using Event.API.Models.DTOs;
using MediatR;

namespace Event.API.Application.Sessions.Queries
{
    public class GetEventSessionsQuery : IRequest<IEnumerable<EventSessionDto>>
    {
        public Guid EventId { get; set; }
    }
}
