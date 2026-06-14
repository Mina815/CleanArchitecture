namespace CleanArchitecture.Application.Services.Queries.GetServiceCategories;

public record GetServiceCategoriesQuery : IRequest<List<ServiceCategoryDto>>;

public class GetServiceCategoriesQueryHandler : IRequestHandler<GetServiceCategoriesQuery, List<ServiceCategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetServiceCategoriesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ServiceCategoryDto>> Handle(GetServiceCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ServiceCategories
            .Where(c => c.IsActive)
            .AsNoTracking()
            .ProjectTo<ServiceCategoryDto>(_mapper.ConfigurationProvider)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
