namespace CleanArchitecture.Application.Branches.Queries.GetCenterBranches;

public record GetCenterBranchesQuery(int CenterId) : IRequest<List<BranchDto>>;

public class GetCenterBranchesQueryHandler : IRequestHandler<GetCenterBranchesQuery, List<BranchDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCenterBranchesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BranchDto>> Handle(GetCenterBranchesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Branches
            .Where(b => b.CenterId == request.CenterId && b.IsActive)
            .AsNoTracking()
            .ProjectTo<BranchDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
