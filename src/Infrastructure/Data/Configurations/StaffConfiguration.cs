namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.Specialization).HasMaxLength(200);
    }
}
