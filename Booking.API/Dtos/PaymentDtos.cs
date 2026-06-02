namespace Booking.API.Dtos
{
    public class PaymentInitiateRequestDto
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
    }

    public class PaymentInitiateResponseDto
    {
        public Guid PaymentId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class PaymentVerifyResponseDto
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public bool IsSuccess { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
}
