using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Staff.Commands.UpdateStaff;

[Authorize(Roles = "Provider")]
public record UpdateStaffCommand : IRequest
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Phone { get; init; }
    public string? ImageUrl { get; init; }
    public string? Specialization { get; init; }
    public bool? IsActive { get; init; }
    public List<int>? ServiceIds { get; init; }
}

public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public UpdateStaffCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await _context.StaffMembers.FindAsync([request.Id], cancellationToken);
        Guard.Against.NotFound(request.Id, staff);

        var branch = await _context.Branches.FindAsync([staff!.BranchId], cancellationToken);
        var center = await _context.BeautyCenters.FindAsync([branch!.CenterId], cancellationToken);
        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        if (request.Name is not null) staff.Name = request.Name;
        if (request.Phone is not null) staff.Phone = request.Phone;
        if (request.ImageUrl is not null) staff.ImageUrl = request.ImageUrl;
        if (request.Specialization is not null) staff.Specialization = request.Specialization;
        if (request.IsActive is not null) staff.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync(cancellationToken);

        if (request.ServiceIds is not null)
        {
            var existing = await _context.StaffServices
                .Where(ss => ss.StaffId == request.Id)
                .ToListAsync(cancellationToken);
            _context.StaffServices.RemoveRange(existing);

            foreach (var serviceId in request.ServiceIds)
            {
                _context.StaffServices.Add(new StaffService { StaffId = request.Id, ServiceId = serviceId });
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
