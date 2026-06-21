namespace CleanArchitecture.Application.Branches.Queries.GetBranchTimeOffs;

public record GetBranchTimeOffsQuery(int BranchId) : IRequest<List<TimeOffDto>>;

public class GetBranchTimeOffsQueryHandler : IRequestHandler<GetBranchTimeOffsQuery, List<TimeOffDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBranchTimeOffsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TimeOffDto>> Handle(GetBranchTimeOffsQuery request, CancellationToken cancellationToken)
    {
        return await _context.TimeOffs
            .Where(t => t.BranchId == request.BranchId && t.StaffId == null)
            .OrderByDescending(t => t.FromDate)
            .Select(t => new TimeOffDto
            {
                Id = t.Id,
                FromDate = t.FromDate,
                ToDate = t.ToDate,
                FromTime = t.FromTime,
                ToTime = t.ToTime,
                Reason = t.Reason,
                Type = t.Type
            })
            .ToListAsync(cancellationToken);
    }
}

public class TimeOffDto
{
    public int Id { get; init; }
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public TimeSpan? FromTime { get; init; }
    public TimeSpan? ToTime { get; init; }
    public string? Reason { get; init; }
    public TimeOffType Type { get; init; }
}
