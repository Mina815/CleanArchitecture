namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class WorkingHourConfiguration : IEntityTypeConfiguration<WorkingHour>
{
    public void Configure(EntityTypeBuilder<WorkingHour> builder)
    {
        builder.Property(x => x.OpenTime).IsRequired();
        builder.Property(x => x.CloseTime).IsRequired();
    }
}
