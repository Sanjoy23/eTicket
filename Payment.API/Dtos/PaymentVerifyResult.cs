namespace Payment.API.Dtos
{
    public class PaymentVerifyResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string RawResponse { get; set; } = string.Empty;
    }
}
