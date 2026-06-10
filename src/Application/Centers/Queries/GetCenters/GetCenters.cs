using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Centers.Queries.GetCenters;

public record GetCentersQuery(string? City, string? Search) : IRequest<List<CenterDto>>;

public class GetCentersQueryHandler : IRequestHandler<GetCentersQuery, List<CenterDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCentersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CenterDto>> Handle(GetCentersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BeautyCenters.AsNoTracking()
            .Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(c => c.Branches.Any(b => b.City == request.City && b.IsActive));

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Name.Contains(request.Search) || c.NameAr.Contains(request.Search));

        var centers = await query.Include(c => c.Branches).ToListAsync(cancellationToken);

        return centers.Select(c =>
        {
            var city = c.Branches.FirstOrDefault(b => b.IsActive)?.City;
            return CenterDto.FromEntity(c, city);
        }).ToList();
    }
}
