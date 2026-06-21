using CleanArchitecture.Application.Centers.Queries.GetCenterById;
using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Centers.Queries.GetMyCenter;

[Authorize(Roles = "Provider")]
public record GetMyCenterQuery : IRequest<CenterDetailDto>;

public class GetMyCenterQueryHandler : IRequestHandler<GetMyCenterQuery, CenterDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _user;

    public GetMyCenterQueryHandler(IApplicationDbContext context, IMapper mapper, IUser user)
    {
        _context = context;
        _mapper = mapper;
        _user = user;
    }

    public async Task<CenterDetailDto> Handle(GetMyCenterQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.BeautyCenters
            .Include(c => c.Branches)
            .Include(c => c.CenterImages.OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(c => c.OwnerId == _user.Id, cancellationToken);

        Guard.Against.NotFound($"Center for user {_user.Id}", entity);

        return _mapper.Map<CenterDetailDto>(entity);
    }
}
