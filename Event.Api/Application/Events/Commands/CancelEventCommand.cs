using MediatR;

namespace Event.API.Application.Commands
{
    public class CancelEventCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }
    }
}
