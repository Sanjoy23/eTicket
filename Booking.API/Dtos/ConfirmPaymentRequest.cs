namespace Booking.API.Dtos
{
    public class ConfirmPaymentRequest
    {
        public Guid PaymentId { get; set; }
        public Guid SessionId { get; set; }
    }
}
