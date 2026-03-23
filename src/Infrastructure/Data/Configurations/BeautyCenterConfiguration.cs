using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BeautyCenterConfiguration : IEntityTypeConfiguration<BeautyCenter>
{
    public void Configure(EntityTypeBuilder<BeautyCenter> builder)
    {
        builder.Property(e => e.OwnerId).HasMaxLength(450).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.DescriptionAr).HasMaxLength(2000);
        builder.Property(e => e.LogoUrl).HasMaxLength(500);
        builder.Property(e => e.AverageRating).HasPrecision(3, 2);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
