using MediatR;
using System;
using System.Collections.Generic;

namespace Event.API.Application.Sessions.Commands
{
    public class ReleaseSessionSeatsCommand : IRequest<Unit>
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<Guid> SeatIds { get; set; } = Array.Empty<Guid>();
    }
}
