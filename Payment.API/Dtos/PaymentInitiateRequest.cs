namespace Payment.API.Dtos
{
    public class PaymentInitiateRequest
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
    }
}
