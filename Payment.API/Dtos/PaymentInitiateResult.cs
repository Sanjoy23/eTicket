namespace Payment.API.Dtos
{
    public class PaymentInitiateResult
    {
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
