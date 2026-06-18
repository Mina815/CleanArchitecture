namespace CleanArchitecture.Application.Centers.Queries.GetCenterById;

public class CenterDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public string? LogoUrl { get; init; }
    public double AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public List<BranchDto> Branches { get; init; } = new();
    public List<string> Images { get; init; } = new();

    public class BranchDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string NameAr { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string? Phone { get; init; }
    }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BeautyCenter, CenterDetailDto>()
                .ForMember(d => d.Images, opt => opt
                    .MapFrom(s => s.CenterImages
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => i.ImageUrl)
                        .ToList()));

            CreateMap<Branch, BranchDto>();
        }
    }
}
