using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Centers.Queries.GetCenterBranches;

public record GetCenterBranchesQuery(int CenterId) : IRequest<List<BranchDetailDto>>;

public class BranchDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string City { get; init; } = null!;
    public string? District { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string Phone { get; init; } = null!;
    public string? WhatsappNumber { get; init; }
    public bool IsActive { get; init; }
    public List<WorkingHourDto> WorkingHours { get; init; } = [];
}

public class WorkingHourDto
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly OpenTime { get; init; }
    public TimeOnly CloseTime { get; init; }
    public bool IsClosed { get; init; }
}

public class GetCenterBranchesQueryHandler : IRequestHandler<GetCenterBranchesQuery, List<BranchDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCenterBranchesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<BranchDetailDto>> Handle(GetCenterBranchesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Branches.AsNoTracking()
            .Where(b => b.CenterId == request.CenterId && b.IsActive)
            .Include(b => b.WorkingHours)
            .Select(b => new BranchDetailDto
            {
                Id = b.Id,
                Name = b.Name,
                NameAr = b.NameAr,
                Address = b.Address,
                City = b.City,
                District = b.District,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                Phone = b.Phone,
                WhatsappNumber = b.WhatsappNumber,
                IsActive = b.IsActive,
                WorkingHours = b.WorkingHours.Select(w => new WorkingHourDto
                {
                    DayOfWeek = w.DayOfWeek,
                    OpenTime = w.OpenTime,
                    CloseTime = w.CloseTime,
                    IsClosed = w.IsClosed
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
