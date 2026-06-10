using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services;

public class BookingValidationService : IBookingValidationService
{
    private readonly IApplicationDbContext _context;
    private readonly IBookingRepository _bookingRepository;

    public BookingValidationService(IApplicationDbContext context, IBookingRepository bookingRepository)
    {
        _context = context;
        _bookingRepository = bookingRepository;
    }

    public async Task ValidateBookingAsync(int branchId, int serviceId, DateOnly bookingDate, TimeOnly startTime, int? staffId, int? excludeBookingId = null, CancellationToken cancellationToken = default)
    {
        var service = await _context.CenterServices
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.IsActive, cancellationToken)
            ?? throw new SlotNotAvailableException("Service is not available.");

        var branch = await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == branchId && b.IsActive, cancellationToken)
            ?? throw new SlotNotAvailableException("Branch is not available.");

        var endTime = startTime.AddMinutes(service.DurationMinutes);

        var workingHour = await _context.WorkingHours
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.BranchId == branchId && w.DayOfWeek == bookingDate.DayOfWeek, cancellationToken);

        if (workingHour is null || workingHour.IsClosed || startTime < workingHour.OpenTime || endTime > workingHour.CloseTime)
            throw new SlotNotAvailableException("Branch is closed at the selected time.");

        if (await _bookingRepository.HasConflictAsync(branchId, bookingDate, startTime, endTime, staffId, excludeBookingId, cancellationToken))
            throw new SlotNotAvailableException();

        var hasTimeOff = await _context.TimeOffs
            .AsNoTracking()
            .AnyAsync(t => t.BranchId == branchId
                && t.FromDate <= bookingDate && t.ToDate >= bookingDate
                && (staffId == null || t.StaffId == null || t.StaffId == staffId)
                && (t.FromTime == null || t.FromTime < endTime)
                && (t.ToTime == null || t.ToTime > startTime), cancellationToken);

        if (hasTimeOff)
            throw new SlotNotAvailableException("Selected time falls within a time-off period.");
    }
}
