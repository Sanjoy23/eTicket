using Microsoft.EntityFrameworkCore;
using ePayment.API.Models;

namespace ePayment.API.Infrastructure.Data
{
    public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
    {
        public DbSet<PaymentEntity> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PaymentEntity>()
                .HasKey(payment => payment.PaymentId);

            modelBuilder.Entity<PaymentEntity>()
                .Property(payment => payment.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PaymentEntity>()
                .Property(payment => payment.Status)
                .HasConversion<int>();

            modelBuilder.Entity<PaymentEntity>()
                .HasIndex(payment => payment.TransactionId)
                .IsUnique();

            modelBuilder.Entity<PaymentEntity>()
                .HasIndex(payment => payment.BookingId);

            modelBuilder.Entity<PaymentEntity>()
                .HasIndex(payment => payment.ReceiptId);

            modelBuilder.Entity<PaymentEntity>()
                .HasIndex(payment => payment.Status);
        }
    }
}
