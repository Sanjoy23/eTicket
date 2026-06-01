using Booking.API.Dtos;
using MediatR;

namespace Booking.API.Application.Queries
{
    public class BookingByIdQuery : IRequest<BookingDto>
    {
        public Guid BookingId { get; set; }
    }
}
