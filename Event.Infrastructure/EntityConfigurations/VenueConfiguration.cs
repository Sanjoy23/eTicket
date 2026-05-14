using Event.Domain.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.Infrastructure.EntityConfigurations
{
    public class VenueConfiguration: IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.ToTable("Venues");

            builder.HasKey(x => x.VenueId);

            builder.Property(x => x.VenueName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Slug)
                .HasMaxLength(200)
                .IsRequired();

            // SEO friendly unique URL
            builder.HasIndex(x => x.Slug)
                .IsUnique()
                .HasDatabaseName("UX_Venues_Slug");

            builder.HasIndex(x => x.City)
                .HasDatabaseName("IX_Venues_City");
        }
    }
}
