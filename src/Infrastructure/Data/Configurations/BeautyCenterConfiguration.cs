using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class BeautyCenterConfiguration : IEntityTypeConfiguration<BeautyCenter>
{
    public void Configure(EntityTypeBuilder<BeautyCenter> builder)
    {
        builder.HasOne<ApplicationUser>()         // no nav property on domain side
            .WithMany()
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
