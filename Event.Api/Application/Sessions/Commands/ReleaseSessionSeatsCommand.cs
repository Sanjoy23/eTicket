using MediatR;
using System;
using System.Collections.Generic;

namespace Event.API.Application.Sessions.Commands
{
    public class ReleaseSessionSeatsCommand : IRequest<Unit>
    {
        public required Guid SessionId { get; set; }
        public required Guid UserId { get; set; }
        public IEnumerable<Guid> SeatIds { get; set; } = [];
    }
}
