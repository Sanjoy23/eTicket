using Event.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.Infrastructure.EntityConfigurations
{
    public class EventSeatInventoryConfiguration : IEntityTypeConfiguration<EventSeatInventory>
    {
        public void Configure(EntityTypeBuilder<EventSeatInventory> builder)
        {
            builder.ToTable("EventSeatInventories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            // Optimistic concurrency
            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // CRITICAL UNIQUE CONSTRAINT
            builder.HasIndex(x => new { x.EventSessionId, x.SeatId })
                .IsUnique()
                .HasDatabaseName(
                    "UX_EventSeatInventory_EventSessionId_SeatId");

            // Fast seat lookup per session
            builder.HasIndex(x => x.EventSessionId)
                .HasDatabaseName(
                    "IX_EventSeatInventory_EventSessionId");

            // Fast status filtering
            builder.HasIndex(x => new
            {
                x.EventSessionId,
                x.Status
            })
            .HasDatabaseName(
                "IX_EventSeatInventory_EventSessionId_Status");

            builder.HasOne(x => x.EventSession)
                .WithMany(x => x.SeatInventories)
                .HasForeignKey(x => x.EventSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Seat)
                .WithMany()
                .HasForeignKey(x => x.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
