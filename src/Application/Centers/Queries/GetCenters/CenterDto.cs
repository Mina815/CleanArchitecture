namespace CleanArchitecture.Application.Centers.Queries.GetCenters;

public class CenterDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public string? City { get; init; }
    public double AverageRating { get; init; }
    public int TotalReviews { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BeautyCenter, CenterDto>()
                .ForMember(d => d.City, opt => opt
                    .MapFrom(s => s.Branches.Select(b => b.City).FirstOrDefault()));
        }
    }
}
