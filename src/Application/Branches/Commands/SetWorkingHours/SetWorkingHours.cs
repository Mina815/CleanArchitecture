using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Branches.Commands.SetWorkingHours;

[Authorize(Roles = Roles.Provider)]
public record SetWorkingHoursCommand : IRequest
{
    public int BranchId { get; init; }
    public List<WorkingHourDto> Hours { get; init; } = [];
}

public record WorkingHourDto(DayOfWeek DayOfWeek, TimeOnly OpenTime, TimeOnly CloseTime, bool IsClosed);

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
        var branch = await _context.Branches.Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == request.BranchId && b.Center.OwnerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.BranchId, branch);

        var existing = await _context.WorkingHours.Where(w => w.BranchId == request.BranchId).ToListAsync(cancellationToken);
        _context.WorkingHours.RemoveRange(existing);

        foreach (var hour in request.Hours)
        {
            _context.WorkingHours.Add(new WorkingHour
            {
                BranchId = request.BranchId,
                DayOfWeek = hour.DayOfWeek,
                OpenTime = hour.OpenTime,
                CloseTime = hour.CloseTime,
                IsClosed = hour.IsClosed
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
