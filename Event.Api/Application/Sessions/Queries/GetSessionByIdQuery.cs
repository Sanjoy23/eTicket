using Event.API.Models.DTOs;
using MediatR;

namespace Event.API.Application.Sessions.Queries
{
    public class GetSessionByIdQuery : IRequest<EventSessionDto>
    {
        public Guid SessionId { get; set; }
    }
}
