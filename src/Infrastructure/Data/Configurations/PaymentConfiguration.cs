namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.TransactionId).HasMaxLength(500);
        builder.Property(x => x.Method).HasMaxLength(100);
        builder.Property(x => x.PaymentUrl).HasMaxLength(1000);
        builder.Property(x => x.ProviderReference).HasMaxLength(500);
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
    }
}
