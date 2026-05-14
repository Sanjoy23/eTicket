using Event.Domain.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.Infrastructure.EntityConfigurations
{
    public class HallConfiguration : IEntityTypeConfiguration<Hall>
    {
        public void Configure(EntityTypeBuilder<Hall> builder)
        {
            builder.ToTable("Halls");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            // Prevent duplicate hall names in same venue
            builder.HasIndex(x => new
            {
                x.VenueId,
                x.Name
            })
            .IsUnique()
            .HasDatabaseName(
                "UX_Halls_VenueId_Name");

            builder.HasIndex(x => x.VenueId)
                .HasDatabaseName(
                    "IX_Halls_VenueId");
        }
    }
}
