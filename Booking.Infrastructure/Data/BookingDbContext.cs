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
        }

        public DbSet<EventBooking> Bookings { get; set; }
        public DbSet<EventBookingSeat> BookingsSeats { get; set; }
    }
}
