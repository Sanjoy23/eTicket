using MediatR;

namespace Event.API.Application.Venues.Commands
{
    public class DeleteVenueCommand : IRequest<Unit>
    {
        public Guid VenueId { get; set; }
    }
}
