namespace CleanArchitecture.Domain.Entities;

public class BeautyCenter : BaseAuditableEntity
{
    public string OwnerId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public string? Description { get; set; }

    public string? DescriptionAr { get; set; }

    public string? LogoUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsVerified { get; set; }

    public decimal AverageRating { get; set; }

    public int TotalReviews { get; set; }

    public IList<Branch> Branches { get; private set; } = new List<Branch>();

    public IList<CenterService> Services { get; private set; } = new List<CenterService>();

    public IList<CenterImage> Images { get; private set; } = new List<CenterImage>();

    public IList<Review> Reviews { get; private set; } = new List<Review>();
}
