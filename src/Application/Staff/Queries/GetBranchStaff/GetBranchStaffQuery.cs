namespace CleanArchitecture.Application.Staff.Queries.GetBranchStaff;

public record GetBranchStaffQuery(int BranchId) : IRequest<List<StaffDto>>;

public class GetBranchStaffQueryHandler : IRequestHandler<GetBranchStaffQuery, List<StaffDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBranchStaffQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StaffDto>> Handle(GetBranchStaffQuery request, CancellationToken cancellationToken)
    {
        return await _context.StaffMembers
            .Where(s => s.BranchId == request.BranchId)
            .OrderBy(s => s.Name)
            .Select(s => new StaffDto
            {
                Id = s.Id,
                BranchId = s.BranchId,
                Name = s.Name,
                Phone = s.Phone,
                ImageUrl = s.ImageUrl,
                Specialization = s.Specialization,
                IsActive = s.IsActive,
                ServiceIds = _context.StaffServices
                    .Where(ss => ss.StaffId == s.Id)
                    .Select(ss => ss.ServiceId)
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }
}

public class StaffDto
{
    public int Id { get; init; }
    public int BranchId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? ImageUrl { get; init; }
    public string? Specialization { get; init; }
    public bool IsActive { get; init; }
    public List<int> ServiceIds { get; init; } = new();
}
