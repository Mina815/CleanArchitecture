using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Queries.GetAvailableSlots;

public record GetAvailableSlotsQuery(int BranchId, int ServiceId, DateOnly Date, int? StaffId = null)
    : IRequest<IReadOnlyList<AvailableSlotDto>>;

public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, IReadOnlyList<AvailableSlotDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAvailableSlotsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AvailableSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var service = await _context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.IsActive, cancellationToken);

        if (service is null)
            return [];

        var branch = await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BranchId && b.IsActive && b.CenterId == service.CenterId, cancellationToken);

        if (branch is null)
            return [];

        if (request.StaffId is int sid)
        {
            var staffOk = await _context.Staff
                .AnyAsync(s => s.Id == sid && s.BranchId == request.BranchId && s.IsActive, cancellationToken);
            if (!staffOk)
                return [];
        }

        var dow = (int)request.Date.DayOfWeek;
        var daySchedule = await _context.WorkingHours
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.BranchId == request.BranchId && w.DayOfWeek == dow, cancellationToken);

        var bookings = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.BranchId == request.BranchId && b.BookingDate == request.Date)
            .ToListAsync(cancellationToken);

        var timeOffs = await _context.TimeOffs
            .AsNoTracking()
            .Where(t => t.BranchId == request.BranchId
                        && t.FromDate <= request.Date
                        && t.ToDate >= request.Date)
            .ToListAsync(cancellationToken);

        var starts = BookingScheduleRules.GetAvailableStarts(
            request.Date,
            service.DurationMinutes,
            daySchedule,
            bookings,
            timeOffs,
            request.StaffId,
            JamalekConstants.DefaultSlotMinutes);

        return starts.Select(t => new AvailableSlotDto { StartTime = t }).ToList();
    }
}
