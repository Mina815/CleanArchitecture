namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BeautyCenterConfiguration : IEntityTypeConfiguration<BeautyCenter>
{
    public void Configure(EntityTypeBuilder<BeautyCenter> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.DescriptionAr).HasMaxLength(2000);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.OwnerId).HasMaxLength(450).IsRequired();
        builder.HasIndex(x => x.OwnerId).IsUnique();

        builder.HasMany(x => x.Branches)
            .WithOne()
            .HasForeignKey(x => x.CenterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.CenterImages)
            .WithOne()
            .HasForeignKey(x => x.CenterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
