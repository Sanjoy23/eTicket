using Event.API.Models.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace Event.API.Application.Sessions.Commands
{
    public class LockSessionSeatsCommand : IRequest<SeatLockResultDto>
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<Guid> SeatIds { get; set; } = Array.Empty<Guid>();
        public int LockDurationMinutes { get; set; } = 15;
    }
}
