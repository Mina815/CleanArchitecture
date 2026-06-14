namespace CleanArchitecture.Application.Centers.Queries.GetCenters;

public record GetCentersQuery : IRequest<PaginatedList<CenterDto>>
{
    public string? City { get; init; }
    public string? Search { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCentersQueryHandler : IRequestHandler<GetCentersQuery, PaginatedList<CenterDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCentersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CenterDto>> Handle(GetCentersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BeautyCenters
            .Where(c => c.IsActive)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(c => c.Branches.Any(b => b.City == request.City));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.NameAr.Contains(search));
        }

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.AverageRating)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<CenterDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedList<CenterDto>(items, count, request.PageNumber, request.PageSize);
    }
}
