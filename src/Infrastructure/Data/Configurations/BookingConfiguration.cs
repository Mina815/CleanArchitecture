using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(e => e.CustomerId).HasMaxLength(450).IsRequired();
        builder.Property(e => e.CustomerNotes).HasMaxLength(1000);
        builder.Property(e => e.CancellationReason).HasMaxLength(500);
        builder.Property(e => e.ServicePrice).HasPrecision(12, 2);
        builder.Property(e => e.TotalAmount).HasPrecision(12, 2);

        builder.HasOne(e => e.Center)
            .WithMany(c => c.Bookings)
            .HasForeignKey(e => e.CenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Branch)
            .WithMany(b => b.Bookings)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Service)
            .WithMany(s => s.Bookings)
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Staff)
            .WithMany(s => s.Bookings)
            .HasForeignKey(e => e.StaffId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Partial unique: no two active bookings start at the same slot for a branch (SQLite filter syntax).
        builder.HasIndex(e => new { e.BranchId, e.BookingDate, e.StartTime })
            .IsUnique()
            .HasFilter($"\"Status\" <> {(int)BookingStatus.Cancelled}");
    }
}
