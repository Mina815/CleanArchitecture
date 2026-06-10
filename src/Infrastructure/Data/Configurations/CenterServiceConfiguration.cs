using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class CenterServiceConfiguration : IEntityTypeConfiguration<CenterService>
{
    public void Configure(EntityTypeBuilder<CenterService> builder)
    {
        builder.ToTable("Services");
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Price).HasPrecision(10, 2);
    }
}
