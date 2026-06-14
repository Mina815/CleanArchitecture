namespace CleanArchitecture.Application.Services.Queries.GetCenterServices;

public record GetCenterServicesQuery(int CenterId) : IRequest<List<ServiceDto>>;

public class GetCenterServicesQueryHandler : IRequestHandler<GetCenterServicesQuery, List<ServiceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCenterServicesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ServiceDto>> Handle(GetCenterServicesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Services
            .Where(s => s.CenterId == request.CenterId && s.IsActive)
            .AsNoTracking()
            .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
