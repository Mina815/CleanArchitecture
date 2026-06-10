using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BeautyCenterConfiguration : IEntityTypeConfiguration<BeautyCenter>
{
    public void Configure(EntityTypeBuilder<BeautyCenter> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(c => c.AverageRating).HasPrecision(3, 2);
        builder.HasIndex(c => c.OwnerId);
    }
}
