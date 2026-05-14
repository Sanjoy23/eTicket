using Event.Domain.Entities.Seating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.Infrastructure.EntityConfigurations
{
    public class SeatLockConfiguration : IEntityTypeConfiguration<SeatLock>
    {
        public void Configure(EntityTypeBuilder<SeatLock> builder)
        {
            builder.ToTable("SeatLocks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.LockedUntilUtc)
                .IsRequired();

            // Prevent duplicate active locks
            builder.HasIndex(x => new
            {
                x.EventSessionId,
                x.SeatId
            })
            .IsUnique()
            .HasDatabaseName(
                "UX_SeatLocks_EventSessionId_SeatId");

            // Cleanup optimization
            builder.HasIndex(x => x.LockedUntilUtc)
                .HasDatabaseName(
                    "IX_SeatLocks_LockedUntilUtc");

            // User lookup
            builder.HasIndex(x => x.UserId)
                .HasDatabaseName(
                    "IX_SeatLocks_UserId");
        }
    }
}
