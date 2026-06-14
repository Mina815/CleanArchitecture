namespace CleanArchitecture.Application.Reviews.Queries.GetCenterReviews;

public class ReviewDto
{
    public int Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTimeOffset Created { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(d => d.CustomerName, opt => opt.Ignore());
        }
    }
}
