using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Domain.Entities.Seating
{
    public class SeatLock
    {
        public Guid Id { get; set; }

        public Guid EventSessionId { get; set; }

        public Guid SeatId { get; set; }

        public Guid UserId { get; set; }

        public DateTime LockedUntilUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
