using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Centers;

public class CenterDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public string? LogoUrl { get; init; }
    public decimal AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public bool IsVerified { get; init; }
    public string? City { get; init; }

    public static CenterDto FromEntity(BeautyCenter c, string? city = null) => new()
    {
        Id = c.Id,
        Name = c.Name,
        NameAr = c.NameAr,
        Description = c.Description,
        DescriptionAr = c.DescriptionAr,
        LogoUrl = c.LogoUrl,
        AverageRating = c.AverageRating,
        TotalReviews = c.TotalReviews,
        IsVerified = c.IsVerified,
        City = city
    };
}

public class CenterDetailDto : CenterDto
{
    public List<BranchSummaryDto> Branches { get; init; } = [];
    public List<ServiceSummaryDto> Services { get; init; } = [];
    public List<string> Images { get; init; } = [];
}

public class BranchSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string City { get; init; } = null!;
    public string Phone { get; init; } = null!;
}

public class ServiceSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
    public string? CategoryName { get; init; }
}
