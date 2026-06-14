namespace CleanArchitecture.Application.Centers.Queries.GetCenterById;

public record GetCenterByIdQuery(int Id) : IRequest<CenterDetailDto>;

public class GetCenterByIdQueryHandler : IRequestHandler<GetCenterByIdQuery, CenterDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCenterByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CenterDetailDto> Handle(GetCenterByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.BeautyCenters
            .AsNoTracking()
            .ProjectTo<CenterDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        return entity;
    }
}
