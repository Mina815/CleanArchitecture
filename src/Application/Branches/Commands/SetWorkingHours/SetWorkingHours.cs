using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using FluentValidation.Results;
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
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);
        Guard.Against.NotFound(request.BranchId, branch);

        if (branch.Center.OwnerId != _user.Id) throw new ForbiddenAccessException();

        var existing = await _context.WorkingHours
            .Where(w => w.BranchId == request.BranchId)
            .ToListAsync(cancellationToken);

        foreach (var hour in request.Hours)
        {
            if (!hour.IsClosed && hour.OpenTime >= hour.CloseTime)
            {
                throw new Common.Exceptions.ValidationException(new[]
                {
                    new ValidationFailure(nameof(hour.DayOfWeek),
                        $"Open time must be before close time for {hour.DayOfWeek}.")
                });
            }

            var existingHour = existing.FirstOrDefault(e => e.DayOfWeek == hour.DayOfWeek);
            if (existingHour is not null)
            {
                existingHour.OpenTime = hour.OpenTime;
                existingHour.CloseTime = hour.CloseTime;
                existingHour.IsClosed = hour.IsClosed;
            }
            else
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
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
