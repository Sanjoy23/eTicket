namespace ePayment.API.Dtos
{
    public class PaymentVerifyResult
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public long ReceiptId { get; set; }
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string RawResponse { get; set; } = string.Empty;
        public string? ValidationId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
