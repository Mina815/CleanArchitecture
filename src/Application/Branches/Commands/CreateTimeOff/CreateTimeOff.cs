using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Branches.Commands.CreateTimeOff;

[Authorize(Roles = Roles.Provider)]
public record CreateTimeOffCommand : IRequest<int>
{
    public int BranchId { get; init; }
    public int? StaffId { get; init; }
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public TimeOnly? FromTime { get; init; }
    public TimeOnly? ToTime { get; init; }
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
        var branch = await _context.Branches.Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == request.BranchId && b.Center.OwnerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.BranchId, branch);

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
