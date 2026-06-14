namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(x => x.CustomerId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(2000);

        builder.HasIndex(x => new { x.BookingId, x.CustomerId }).IsUnique();
    }
}
