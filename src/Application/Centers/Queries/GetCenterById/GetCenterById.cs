using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Centers.Queries.GetCenterById;

public record GetCenterByIdQuery(int Id) : IRequest<CenterDetailDto?>;

public class GetCenterByIdQueryHandler : IRequestHandler<GetCenterByIdQuery, CenterDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCenterByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<CenterDetailDto?> Handle(GetCenterByIdQuery request, CancellationToken cancellationToken)
    {
        var center = await _context.BeautyCenters.AsNoTracking()
            .Include(c => c.Branches)
            .Include(c => c.Services).ThenInclude(s => s.Category)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive, cancellationToken);

        if (center is null) return null;

        return new CenterDetailDto
        {
            Id = center.Id,
            Name = center.Name,
            NameAr = center.NameAr,
            Description = center.Description,
            DescriptionAr = center.DescriptionAr,
            LogoUrl = center.LogoUrl,
            AverageRating = center.AverageRating,
            TotalReviews = center.TotalReviews,
            IsVerified = center.IsVerified,
            City = center.Branches.FirstOrDefault(b => b.IsActive)?.City,
            Branches = center.Branches.Where(b => b.IsActive).Select(b => new BranchSummaryDto
            {
                Id = b.Id, Name = b.Name, NameAr = b.NameAr, Address = b.Address, City = b.City, Phone = b.Phone
            }).ToList(),
            Services = center.Services.Where(s => s.IsActive).Select(s => new ServiceSummaryDto
            {
                Id = s.Id, Name = s.Name, NameAr = s.NameAr, Price = s.Price,
                DurationMinutes = s.DurationMinutes, CategoryName = s.Category?.Name
            }).ToList(),
            Images = center.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()
        };
    }
}
