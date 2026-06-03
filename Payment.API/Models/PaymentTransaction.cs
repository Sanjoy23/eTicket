namespace ePayment.API.Models
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OrderId { get; set; } = string.Empty;
        public string? SessionKey { get; set; }
        public string? ValidationId { get; set; }
        public string? BankTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? FailureReason { get; set; }
        public string? CardType { get; set; }
        public string? RiskLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed,
        Cancelled,
        Validated
    }
}
