namespace CleanArchitecture.Application.Reviews.Queries.GetCenterReviews;

public record GetCenterReviewsQuery : IRequest<PaginatedList<ReviewDto>>
{
    public int CenterId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCenterReviewsQueryHandler : IRequestHandler<GetCenterReviewsQuery, PaginatedList<ReviewDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCenterReviewsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ReviewDto>> Handle(GetCenterReviewsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Reviews
            .Where(r => r.CenterId == request.CenterId && r.IsApproved)
            .AsNoTracking();

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.Created)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedList<ReviewDto>(items, count, request.PageNumber, request.PageSize);
    }
}
