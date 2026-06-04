using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Domain.Entities
{
    public class Receipt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public DateTime PaymentDate { get; set; }

        public int PaymentType { get; set; }

        [MaxLength(20)]
        public string ChequeNumber { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string PaymentInfo { get; set; } = string.Empty;

        [Column("payment_amount", TypeName = "numeric(18,2)")]
        public decimal PaymentAmount { get; set; }

        [MaxLength(50)]
        public string CurrencyId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string PaperNumber { get; set; } = string.Empty;

        [MaxLength(250)]
        public string AmountString { get; set; } = string.Empty;

        [MaxLength(250)]
        public string TransactionResultText { get; set; } = string.Empty;
        public Guid TransactionId { get; set; }
        public string QrCodeContentString { get; set; } = string.Empty;
        public byte[] QrCodeContent { get; set; } = [];
        public bool IsPaid { get; set; }
        [MaxLength(50)]
        public string ModifiedIp { get; set; } = string.Empty;
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }

        [MaxLength(50)]
        public string CreatedIp { get; set; } = string.Empty;

        [MaxLength(100)]
        public string GatewayPageUrl { get; set; } = string.Empty;
    }
}
