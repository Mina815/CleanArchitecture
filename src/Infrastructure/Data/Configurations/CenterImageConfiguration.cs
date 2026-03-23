using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class CenterImageConfiguration : IEntityTypeConfiguration<CenterImage>
{
    public void Configure(EntityTypeBuilder<CenterImage> builder)
    {
        builder.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Caption).HasMaxLength(500);

        builder.HasOne(e => e.Center)
            .WithMany(c => c.Images)
            .HasForeignKey(e => e.CenterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
