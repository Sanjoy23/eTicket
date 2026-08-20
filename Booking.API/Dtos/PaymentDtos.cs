namespace Booking.API.Dtos
{
    public class PaymentInitiateRequestDto
    {
        public long Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
        public string ReceiptNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string ProductProfile { get; set; } = string.Empty;
    }

    public class PaymentInitiateResponseDto
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class PaymentVerifyResponseDto
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public long ReceiptId { get; set; }
        public bool IsSuccess { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
