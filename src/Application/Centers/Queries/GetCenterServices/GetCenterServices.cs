using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Centers.Queries.GetCenterServices;

public record GetCenterServicesQuery(int CenterId, int? CategoryId = null) : IRequest<List<ServiceDetailDto>>;

public class ServiceDetailDto
{
    public int Id { get; init; }
    public int CategoryId { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
    public string? ImageUrl { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public string? CategoryName { get; init; }
}

public class GetCenterServicesQueryHandler : IRequestHandler<GetCenterServicesQuery, List<ServiceDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCenterServicesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ServiceDetailDto>> Handle(GetCenterServicesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<CenterService> query = _context.CenterServices.AsNoTracking()
            .Include(s => s.Category)
            .Where(s => s.CenterId == request.CenterId && s.IsActive);

        if (request.CategoryId.HasValue)
            query = query.Where(s => s.CategoryId == request.CategoryId.Value);

        return await query
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new ServiceDetailDto
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                Name = s.Name,
                NameAr = s.NameAr,
                Description = s.Description,
                DescriptionAr = s.DescriptionAr,
                Price = s.Price,
                DurationMinutes = s.DurationMinutes,
                ImageUrl = s.ImageUrl,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive,
                CategoryName = s.Category != null ? s.Category.Name : null
            })
            .ToListAsync(cancellationToken);
    }
}
