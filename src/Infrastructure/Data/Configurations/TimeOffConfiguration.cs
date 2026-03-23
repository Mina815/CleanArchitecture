using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class TimeOffConfiguration : IEntityTypeConfiguration<TimeOff>
{
    public void Configure(EntityTypeBuilder<TimeOff> builder)
    {
        builder.Property(e => e.Reason).HasMaxLength(500);

        builder.HasOne(e => e.Branch)
            .WithMany(b => b.TimeOffs)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Staff)
            .WithMany(s => s.TimeOffs)
            .HasForeignKey(e => e.StaffId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
