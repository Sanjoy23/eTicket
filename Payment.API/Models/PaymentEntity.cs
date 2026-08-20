namespace ePayment.API.Models
{
    public class PaymentEntity
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public long ReceiptId { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string TransactionId { get; set; } = string.Empty;
        public string SessionKey { get; set; } = string.Empty;
        public string GatewayPaymentUrl { get; set; } = string.Empty;
        public string FailureReason { get; set; } = string.Empty;
        public string ValidationId { get; set; } = string.Empty;
        public string BankTransactionId { get; set; } = string.Empty;
        public string RawGatewayResponse { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
