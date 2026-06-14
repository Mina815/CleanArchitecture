using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Branches.Commands.SetWorkingHours;

[Authorize(Roles = "Provider")]
public record SetWorkingHoursCommand : IRequest
{
    public int BranchId { get; init; }
    public List<WorkingHourDto> WorkingHours { get; init; } = new();
}

public record WorkingHourDto
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeSpan OpenTime { get; init; }
    public TimeSpan CloseTime { get; init; }
    public bool IsClosed { get; init; }
}

public class SetWorkingHoursCommandHandler : IRequestHandler<SetWorkingHoursCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public SetWorkingHoursCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(SetWorkingHoursCommand request, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches
            .FindAsync([request.BranchId], cancellationToken);

        Guard.Against.NotFound(request.BranchId, branch);

        var center = await _context.BeautyCenters
            .FindAsync([branch!.CenterId], cancellationToken);

        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        var existingHours = await _context.WorkingHours
            .Where(w => w.BranchId == request.BranchId)
            .ToListAsync(cancellationToken);

        _context.WorkingHours.RemoveRange(existingHours);

        foreach (var dto in request.WorkingHours)
        {
            _context.WorkingHours.Add(new WorkingHour
            {
                BranchId = request.BranchId,
                DayOfWeek = dto.DayOfWeek,
                OpenTime = dto.OpenTime,
                CloseTime = dto.CloseTime,
                IsClosed = dto.IsClosed
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
