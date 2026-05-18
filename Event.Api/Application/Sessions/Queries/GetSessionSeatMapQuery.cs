using Event.API.Models.DTOs;
using MediatR;

namespace Event.API.Application.Sessions.Queries
{
    public class GetSessionSeatMapQuery : IRequest<SessionSeatMapDto>
    {
        public Guid SessionId { get; set; }
    }
}
