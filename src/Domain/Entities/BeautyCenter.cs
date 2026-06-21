namespace CleanArchitecture.Domain.Entities;

public class BeautyCenter : BaseAuditableEntity
{
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public bool IsProfileComplete =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(NameAr) &&
        !string.IsNullOrWhiteSpace(Description) &&
        !string.IsNullOrWhiteSpace(DescriptionAr) &&
        !string.IsNullOrWhiteSpace(LogoUrl);

    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<CenterImage> CenterImages { get; set; } = new List<CenterImage>();
}
