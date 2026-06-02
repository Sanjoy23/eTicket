using MediatR;

namespace Booking.API.Application.Commands
{
    public class BookSeatsCommand : IRequest<BookSeatsResponse>
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public Guid SessionId { get; set; }
        public IEnumerable<Guid> SeatIds { get; set; } = [];
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "BDT";
    }

    public class BookSeatsResponse
    {
        public Guid BookingId { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
