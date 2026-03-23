using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(32).IsRequired();
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.Specialization).HasMaxLength(200);

        builder.HasOne(e => e.Branch)
            .WithMany(b => b.Staff)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
