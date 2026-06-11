namespace ePayment.API.Dtos
{
    public class PaymentInfoDto
    {
        public long Id { get; set; }
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
}
