namespace CleanArchitecture.Application.BeautyCenters.Queries.GetCenters;

public class CenterBriefDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string NameAr { get; init; } = string.Empty;

    public string? LogoUrl { get; init; }

    public decimal AverageRating { get; init; }

    public int TotalReviews { get; init; }

    /// <summary>One representative city from an active branch (for list display).</summary>
    public string? City { get; init; }
}
