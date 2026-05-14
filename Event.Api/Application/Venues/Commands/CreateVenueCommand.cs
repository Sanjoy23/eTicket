using MediatR;

namespace Event.API.Application.Venues.Commands
{
    public class CreateVenueCommand : IRequest<Guid>
    {
        public string VenueName { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
