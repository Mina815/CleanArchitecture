using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.Property(b => b.Name).HasMaxLength(200).IsRequired();
        builder.Property(b => b.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(b => b.Address).HasMaxLength(500).IsRequired();
        builder.Property(b => b.City).HasMaxLength(100).IsRequired();
        builder.Property(b => b.Phone).HasMaxLength(20).IsRequired();
        builder.HasIndex(b => b.City);
    }
}
