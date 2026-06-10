using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.ServicePrice).HasPrecision(10, 2);
        builder.Property(b => b.TotalAmount).HasPrecision(10, 2);
        builder.HasIndex(b => new { b.BranchId, b.BookingDate, b.StartTime })
            .IsUnique()
            .HasFilter($"\"{nameof(Booking.Status)}\" != {(int)BookingStatus.Cancelled}");
        builder.HasOne(b => b.Payment).WithOne(p => p.Booking).HasForeignKey<Payment>(p => p.BookingId);
        builder.HasOne(b => b.Review).WithOne(r => r.Booking).HasForeignKey<Review>(r => r.BookingId);
    }
}
