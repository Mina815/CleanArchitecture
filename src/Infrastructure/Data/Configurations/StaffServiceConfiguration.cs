namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class StaffServiceConfiguration : IEntityTypeConfiguration<StaffService>
{
    public void Configure(EntityTypeBuilder<StaffService> builder)
    {
        builder.ToTable("StaffServices");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.StaffId, x.ServiceId }).IsUnique();
    }
}
