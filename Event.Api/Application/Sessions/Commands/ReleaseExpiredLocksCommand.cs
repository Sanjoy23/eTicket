using MediatR;

namespace Event.API.Application.Sessions.Commands
{
    public class ReleaseExpiredLocksCommand: IRequest<Unit>
    {
    }
}
