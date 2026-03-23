using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.BeautyCenters.Queries.GetCenters;

public record GetCentersQuery(string? City = null) : IRequest<IReadOnlyList<CenterBriefDto>>;

public class GetCentersQueryHandler : IRequestHandler<GetCentersQuery, IReadOnlyList<CenterBriefDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCentersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CenterBriefDto>> Handle(GetCentersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BeautyCenters
            .AsNoTracking()
            .Where(c => c.IsActive && c.Branches.Any(b => b.IsActive));

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim();
            query = query.Where(c => c.Branches.Any(b => b.IsActive && b.City == city));
        }

        var list = await query
            .OrderBy(c => c.Name)
            .Select(c => new CenterBriefDto
            {
                Id = c.Id,
                Name = c.Name,
                NameAr = c.NameAr,
                LogoUrl = c.LogoUrl,
                AverageRating = c.AverageRating,
                TotalReviews = c.TotalReviews,
                City = c.Branches.Where(b => b.IsActive).OrderBy(b => b.Id).Select(b => b.City).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return list;
    }
}
