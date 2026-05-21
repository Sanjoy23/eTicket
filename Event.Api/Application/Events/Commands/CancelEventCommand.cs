using MediatR;

namespace Event.API.Application.Events.Commands
{
    public class CancelEventCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }
    }
}
