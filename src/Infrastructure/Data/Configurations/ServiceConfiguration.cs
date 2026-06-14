namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.DescriptionAr).HasMaxLength(2000);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
    }
}
