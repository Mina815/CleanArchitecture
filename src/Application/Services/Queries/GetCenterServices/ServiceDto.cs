namespace CleanArchitecture.Application.Services.Queries.GetCenterServices;

public class ServiceDto
{
    public int Id { get; init; }
    public int CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
    public string? ImageUrl { get; init; }
    public int DisplayOrder { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Service, ServiceDto>();
        }
    }
}
