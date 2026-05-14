using Event.Domain.Enums;
using Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Data
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EventEntity>()
            .HasOne(e => e.Venue)
            .WithMany(v => v.Events)
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventPerformer>()
            .HasKey(ep => new { ep.EventId, ep.PerformerId });

            modelBuilder.Entity<EventPerformer>()
            .HasOne(ep => ep.Event)
            .WithMany(e => e.EventPerformers)
            .HasForeignKey(ep => ep.EventId);

            modelBuilder.Entity<EventPerformer>()
            .HasOne(ep => ep.Performer)
            .WithMany(p => p.EventPerformers)
            .HasForeignKey(ep => ep.PerformerId);

            modelBuilder.Entity<EventEntity>()
                .Property(e => e.Type)
                .HasConversion<string>()
                .HasDefaultValue(EventType.NotAdded);
            modelBuilder.Entity<EventEntity>()
                .Property(e => e.Status)
                .HasConversion<string>()
                .HasDefaultValue(EventStatus.Upcoming);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Venue)
                .WithMany(v => v.Seats)
                .HasForeignKey(s => s.VenueId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<EventSeat>()
                .HasOne(es => es.Event)
                .WithMany(e => e.EventSeats)
                .HasForeignKey(es => es.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventSeat>()
                .HasOne(es => es.Seat)
                .WithMany()
                .HasForeignKey(es => es.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.EventSeat)
                .WithMany()
                .HasForeignKey(bs => bs.EventSeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventSeat>()
                .Property(es => es.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasColumnType("decimal(18,2)");


        }

        private static void ConfigureVenue(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Seats)
                .WithOne(e => e.Venue)
                .HasForeignKey(s => s.VenueId);

            modelBuilder.Entity<Venue>()
            .HasMany(v => v.Events)
            .WithOne(e => e.Venue)
            .HasForeignKey(e => e.VenueId);
        }
        private static void ConfigureEvent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventEntity>()
                .HasMany(e => e.EventSeats)
                .WithOne(es => es.Event)
                .HasForeignKey(es => es.EventId);
        }


        public DbSet<EventEntity> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<EventPerformer> EventPerformers { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<EventSeat> EventSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingsSeats { get; set; }
    }
}
