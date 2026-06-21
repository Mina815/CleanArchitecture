using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Staff.Commands.DeleteStaff;

[Authorize(Roles = "Provider")]
public record DeleteStaffCommand(int Id) : IRequest;

public class DeleteStaffCommandHandler : IRequestHandler<DeleteStaffCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public DeleteStaffCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await _context.StaffMembers.FindAsync([request.Id], cancellationToken);
        Guard.Against.NotFound(request.Id, staff);

        var branch = await _context.Branches.FindAsync([staff!.BranchId], cancellationToken);
        var center = await _context.BeautyCenters.FindAsync([branch!.CenterId], cancellationToken);
        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        var hasBookings = await _context.Bookings
            .AnyAsync(b => b.StaffId == request.Id && b.Status != BookingStatus.Cancelled, cancellationToken);

        if (hasBookings)
        {
            staff.IsActive = false;
        }
        else
        {
            var staffServices = await _context.StaffServices
                .Where(ss => ss.StaffId == request.Id)
                .ToListAsync(cancellationToken);
            _context.StaffServices.RemoveRange(staffServices);
            _context.StaffMembers.Remove(staff);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
