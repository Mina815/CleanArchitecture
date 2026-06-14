namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(500).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();
        builder.Property(x => x.District).HasMaxLength(100);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.WhatsappNumber).HasMaxLength(50);

        builder.HasMany<WorkingHour>()
            .WithOne()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<TimeOff>()
            .WithOne()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
