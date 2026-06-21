using CleanArchitecture.Application.Branches.Commands.SetWorkingHours;

namespace CleanArchitecture.Application.Branches.Queries.GetBranchWorkingHours;

public record GetBranchWorkingHoursQuery(int BranchId) : IRequest<List<WorkingHourDto>>;

public class GetBranchWorkingHoursQueryHandler : IRequestHandler<GetBranchWorkingHoursQuery, List<WorkingHourDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBranchWorkingHoursQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkingHourDto>> Handle(GetBranchWorkingHoursQuery request, CancellationToken cancellationToken)
    {
        return await _context.WorkingHours
            .Where(w => w.BranchId == request.BranchId)
            .OrderBy(w => w.DayOfWeek)
            .Select(w => new WorkingHourDto
            {
                DayOfWeek = w.DayOfWeek,
                OpenTime = w.OpenTime,
                CloseTime = w.CloseTime,
                IsClosed = w.IsClosed
            })
            .ToListAsync(cancellationToken);
    }
}
