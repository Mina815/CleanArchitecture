namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class TimeOffConfiguration : IEntityTypeConfiguration<TimeOff>
{
    public void Configure(EntityTypeBuilder<TimeOff> builder)
    {
        builder.Property(x => x.Reason).HasMaxLength(500);
    }
}
