namespace CleanArchitecture.Application.ServiceCategories.Queries.GetServiceCategories;

public class ServiceCategoryDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string NameAr { get; init; } = string.Empty;

    public string? IconUrl { get; init; }

    public int DisplayOrder { get; init; }
}
