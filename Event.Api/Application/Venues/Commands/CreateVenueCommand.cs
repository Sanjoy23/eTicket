using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Event.API.Application.Venues.Commands
{
    public class CreateVenueCommand : IRequest<int>
    {
        public string VenueName { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Address { get; set; }
    }
}
