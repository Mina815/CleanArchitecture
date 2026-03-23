using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(e => e.TransactionId).HasMaxLength(128);
        builder.Property(e => e.Currency).HasMaxLength(8).IsRequired();
        builder.Property(e => e.Method).HasMaxLength(64);
        builder.Property(e => e.PaymentUrl).HasMaxLength(2000);
        builder.Property(e => e.ProviderReference).HasMaxLength(256);
        builder.Property(e => e.FailureReason).HasMaxLength(1000);
        builder.Property(e => e.Amount).HasPrecision(12, 2);

        builder.HasOne(e => e.Booking)
            .WithOne(b => b.Payment)
            .HasForeignKey<Payment>(e => e.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
