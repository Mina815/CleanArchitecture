using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Services.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class CategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string? IconUrl { get; init; }
}

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ServiceCategories.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, NameAr = c.NameAr, IconUrl = c.IconUrl })
            .ToListAsync(cancellationToken);
    }
}
