using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Staff.Commands.SetStaffServices;

[Authorize(Roles = "Provider")]
public record SetStaffServicesCommand : IRequest
{
    public int StaffId { get; init; }
    public List<int> ServiceIds { get; init; } = new();
}

public class SetStaffServicesCommandHandler : IRequestHandler<SetStaffServicesCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public SetStaffServicesCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(SetStaffServicesCommand request, CancellationToken cancellationToken)
    {
        var staff = await _context.StaffMembers.FindAsync([request.StaffId], cancellationToken);
        Guard.Against.NotFound(request.StaffId, staff);

        var branch = await _context.Branches.FindAsync([staff!.BranchId], cancellationToken);
        var center = await _context.BeautyCenters.FindAsync([branch!.CenterId], cancellationToken);
        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        var existing = await _context.StaffServices
            .Where(ss => ss.StaffId == request.StaffId)
            .ToListAsync(cancellationToken);
        _context.StaffServices.RemoveRange(existing);

        foreach (var serviceId in request.ServiceIds)
        {
            _context.StaffServices.Add(new StaffService { StaffId = request.StaffId, ServiceId = serviceId });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
