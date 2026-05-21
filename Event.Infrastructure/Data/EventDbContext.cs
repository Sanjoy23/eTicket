using Event.Domain.Enums;
using Event.Domain.Entities;
using Event.Domain.Entities.Events;
using Event.Domain.Entities.Seating;
using Event.Domain.Entities.Ticketing;
using Event.Domain.Entities.Venues;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Data
{
    public class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventDbContext).Assembly);

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

            modelBuilder.Entity<EventSession>()
                .HasOne(es => es.Event)
                .WithMany()
                .HasForeignKey(es => es.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventSession>()
                .HasOne(es => es.Venue)
                .WithMany()
                .HasForeignKey(es => es.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventSession>()
                .HasOne(es => es.Hall)
                .WithMany()
                .HasForeignKey(es => es.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<BookingSeat>()
            //    .HasOne(bs => bs.Booking)
            //    .WithMany(b => b.BookingSeats)
            //    .HasForeignKey(bs => bs.BookingId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<BookingSeat>()
            //    .HasOne(bs => bs.EventSeat)
            //    .WithMany()
            //    .HasForeignKey(bs => bs.EventSeatId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventSeat>()
                .Property(es => es.Price)
                .HasColumnType("decimal(18,2)");

            //modelBuilder.Entity<Booking>()
            //    .Property(b => b.TotalAmount)
            //    .HasColumnType("decimal(18,2)");


        }

        public DbSet<EventEntity> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<EventPerformer> EventPerformers { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<EventSession> EventSessions { get; set; }
        public DbSet<EventSeat> EventSeats { get; set; }
        public DbSet<EventSeatInventory> EventSeatInventories { get; set; }
        public DbSet<SeatLock> SeatLocks { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
    }
}
