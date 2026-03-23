using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Address).HasMaxLength(500).IsRequired();
        builder.Property(e => e.City).HasMaxLength(120).IsRequired();
        builder.Property(e => e.District).HasMaxLength(120).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(32).IsRequired();
        builder.Property(e => e.WhatsappNumber).HasMaxLength(32);

        builder.HasOne(e => e.Center)
            .WithMany(c => c.Branches)
            .HasForeignKey(e => e.CenterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
