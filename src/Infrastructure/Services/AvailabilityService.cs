using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services;

public class AvailabilityService : IAvailabilityService
{
    private const int SlotDurationMinutes = 30;
    private readonly IApplicationDbContext _context;

    public AvailabilityService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TimeSlotDto>> GetAvailableSlotsAsync(int branchId, int serviceId, DateOnly date, int? staffId = null, CancellationToken cancellationToken = default)
    {
        var service = await _context.CenterServices
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.IsActive, cancellationToken);

        if (service is null) return [];

        var workingHour = await _context.WorkingHours
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.BranchId == branchId && w.DayOfWeek == date.DayOfWeek, cancellationToken);

        if (workingHour is null || workingHour.IsClosed) return [];

        var bookings = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.BranchId == branchId && b.BookingDate == date && b.Status != BookingStatus.Cancelled)
            .ToListAsync(cancellationToken);

        var timeOffs = await _context.TimeOffs
            .AsNoTracking()
            .Where(t => t.BranchId == branchId
                && t.FromDate <= date && t.ToDate >= date
                && (staffId == null || t.StaffId == null || t.StaffId == staffId))
            .ToListAsync(cancellationToken);

        var slots = new List<TimeSlotDto>();
        var current = workingHour.OpenTime;

        while (current.AddMinutes(service.DurationMinutes) <= workingHour.CloseTime)
        {
            var end = current.AddMinutes(service.DurationMinutes);
            var isAvailable = true;

            foreach (var booking in bookings)
            {
                if (staffId.HasValue && booking.StaffId.HasValue && booking.StaffId != staffId) continue;
                if (current < booking.EndTime && end > booking.StartTime)
                {
                    isAvailable = false;
                    break;
                }
            }

            if (isAvailable)
            {
                foreach (var timeOff in timeOffs)
                {
                    var offStart = timeOff.FromTime ?? TimeOnly.MinValue;
                    var offEnd = timeOff.ToTime ?? TimeOnly.MaxValue;
                    if (current < offEnd && end > offStart)
                    {
                        isAvailable = false;
                        break;
                    }
                }
            }

            slots.Add(new TimeSlotDto(current, end, isAvailable));
            current = current.AddMinutes(SlotDurationMinutes);
        }

        return slots.Where(s => s.IsAvailable).ToList();
    }
}
