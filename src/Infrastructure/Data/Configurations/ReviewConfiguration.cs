using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(e => e.CustomerId).HasMaxLength(450).IsRequired();
        builder.Property(e => e.Comment).HasMaxLength(2000);

        builder.HasIndex(e => e.BookingId).IsUnique();

        builder.HasOne(e => e.Center)
            .WithMany(c => c.Reviews)
            .HasForeignKey(e => e.CenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Booking)
            .WithOne(b => b.Review)
            .HasForeignKey<Review>(e => e.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
