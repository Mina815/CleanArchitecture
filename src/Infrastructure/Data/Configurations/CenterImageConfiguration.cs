namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class CenterImageConfiguration : IEntityTypeConfiguration<CenterImage>
{
    public void Configure(EntityTypeBuilder<CenterImage> builder)
    {
        builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Caption).HasMaxLength(500);
    }
}
