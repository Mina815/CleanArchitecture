using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Reviews.Queries.GetCenterReviews;

public record GetCenterReviewsQuery(int CenterId, int Page = 1, int PageSize = 10) : IRequest<ReviewsVm>;

public class ReviewDto
{
    public int Id { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTimeOffset Created { get; init; }
}

public class ReviewsVm
{
    public List<ReviewDto> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public class GetCenterReviewsQueryHandler : IRequestHandler<GetCenterReviewsQuery, ReviewsVm>
{
    private readonly IApplicationDbContext _context;

    public GetCenterReviewsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ReviewsVm> Handle(GetCenterReviewsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Reviews.AsNoTracking()
            .Where(r => r.CenterId == request.CenterId && r.IsApproved);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.Created)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewDto { Id = r.Id, Rating = r.Rating, Comment = r.Comment, Created = r.Created })
            .ToListAsync(cancellationToken);

        return new ReviewsVm { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
    }
}
