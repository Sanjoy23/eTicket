namespace Payment.API.Dtos
{
    public class PaymentVerifyResponse
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public bool IsSuccess { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
}
