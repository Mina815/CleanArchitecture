using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Staff.Queries.GetBranchStaff;

public record GetBranchStaffQuery(int BranchId) : IRequest<List<StaffDto>>;

public class GetBranchStaffQueryHandler : IRequestHandler<GetBranchStaffQuery, List<StaffDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBranchStaffQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<StaffDto>> Handle(GetBranchStaffQuery request, CancellationToken cancellationToken)
    {
        var staff = await _context.StaffMembers.AsNoTracking()
            .Where(s => s.BranchId == request.BranchId && s.IsActive)
            .ToListAsync(cancellationToken);

        return staff.Select(StaffDto.FromEntity).ToList();
    }
}
