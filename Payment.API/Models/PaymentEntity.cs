namespace Payment.API.Models
{
    public class PaymentEntity
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        //Provider = request.Provider,
        public int Status { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string GatewayPaymentUrl { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
