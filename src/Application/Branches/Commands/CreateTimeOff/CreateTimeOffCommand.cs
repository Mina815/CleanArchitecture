using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Branches.Commands.CreateTimeOff;

[Authorize(Roles = "Provider")]
public record CreateTimeOffCommand : IRequest<int>
{
    public int BranchId { get; init; }
    public int? StaffId { get; init; }
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public TimeSpan? FromTime { get; init; }
    public TimeSpan? ToTime { get; init; }
    public string? Reason { get; init; }
    public TimeOffType Type { get; init; }
}

public class CreateTimeOffCommandHandler : IRequestHandler<CreateTimeOffCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateTimeOffCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateTimeOffCommand request, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches
            .FindAsync([request.BranchId], cancellationToken);

        Guard.Against.NotFound(request.BranchId, branch);

        var center = await _context.BeautyCenters
            .FindAsync([branch!.CenterId], cancellationToken);

        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        var entity = new TimeOff
        {
            BranchId = request.BranchId,
            StaffId = request.StaffId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            FromTime = request.FromTime,
            ToTime = request.ToTime,
            Reason = request.Reason,
            Type = request.Type
        };

        _context.TimeOffs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
