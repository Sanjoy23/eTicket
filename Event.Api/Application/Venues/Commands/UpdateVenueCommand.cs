using MediatR;

namespace Event.API.Application.Venues.Commands
{
    public class UpdateVenueCommand : IRequest<Unit>
    {
        public required Guid VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public required int Capacity { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
