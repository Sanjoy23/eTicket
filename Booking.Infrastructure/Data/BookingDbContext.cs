using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Data
{
    public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventBooking>()
                .Property(booking => booking.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<EventBooking>()
                .HasMany(booking => booking.BookingSeats)
                .WithOne(bookingSeat => bookingSeat.Booking)
                .HasForeignKey(bookingSeat => bookingSeat.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Receipt>(entity =>
            {
                
                entity.Property(r => r.PaymentDate)
                    .HasColumnType("timestamp with time zone");

                entity.Property(r => r.ModifiedOn)
                    .HasColumnType("timestamp with time zone");

                entity.Property(r => r.CreatedOn)
                    .HasColumnType("timestamp with time zone");

                entity.Property(r => r.QrCodeContent)
                    .HasColumnType("bytea");

                entity.HasIndex(r => r.ReceiptNumber)
                    .IsUnique();

                entity.HasIndex(r => r.TransactionId);
                entity.HasIndex(r => r.UserId);
                entity.HasIndex(r => r.EventId);
                entity.HasIndex(r => r.IsPaid);
            });
        }

        public DbSet<EventBooking> Bookings { get; set; }
        public DbSet<EventBookingSeat> BookingsSeats { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
    }
}
