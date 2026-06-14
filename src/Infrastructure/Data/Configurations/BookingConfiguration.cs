namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(x => x.CustomerId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.CustomerNotes).HasMaxLength(1000);
        builder.Property(x => x.CancellationReason).HasMaxLength(500);
        builder.Property(x => x.ServicePrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();

        builder.HasIndex(x => new { x.BranchId, x.BookingDate, x.StartTime })
            .IsUnique()
            .HasFilter("\"Status\" NOT IN (3, 4)");
    }
}
