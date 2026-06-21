using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Staff.Commands.CreateStaff;

[Authorize(Roles = "Provider")]
public record CreateStaffCommand : IRequest<int>
{
    public int BranchId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? ImageUrl { get; init; }
    public string? Specialization { get; init; }
    public List<int> ServiceIds { get; init; } = new();
}

public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateStaffCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches.FindAsync([request.BranchId], cancellationToken);
        Guard.Against.NotFound(request.BranchId, branch);

        var center = await _context.BeautyCenters.FindAsync([branch!.CenterId], cancellationToken);
        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        var staff = new StaffEntity
        {
            BranchId = request.BranchId,
            Name = request.Name,
            Phone = request.Phone,
            ImageUrl = request.ImageUrl,
            Specialization = request.Specialization
        };

        _context.StaffMembers.Add(staff);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var serviceId in request.ServiceIds)
        {
            _context.StaffServices.Add(new StaffService { StaffId = staff.Id, ServiceId = serviceId });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return staff.Id;
    }
}
