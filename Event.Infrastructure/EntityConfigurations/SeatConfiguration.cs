using Event.Domain.Entities.Seating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.Infrastructure.EntityConfigurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("Seats");

            builder.HasKey(x => x.SeatId);

            builder.Property(x => x.SeatCode)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.RowLabel)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.XPosition)
                .HasPrecision(10, 2);

            builder.Property(x => x.YPosition)
                .HasPrecision(10, 2);

            builder.HasIndex(x => new { x.HallId, x.SeatCode })
                .IsUnique()
                .HasDatabaseName("UX_Seats_HallId_SeatCode");

            builder.HasIndex(x => x.HallId)
                .HasDatabaseName("IX_Seats_HallId");

            builder.HasOne(x => x.Hall)
                .WithMany(x => x.Seats)
                .HasForeignKey(x => x.HallId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
