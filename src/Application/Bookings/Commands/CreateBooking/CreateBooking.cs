using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Scheduling;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Commands.CreateBooking;

[Authorize(Roles = Roles.Customer)]
public record CreateBookingCommand : IRequest<int>
{
    public int BranchId { get; init; }

    public int ServiceId { get; init; }

    public int? StaffId { get; init; }

    public DateOnly BookingDate { get; init; }

    public TimeOnly StartTime { get; init; }

    public string? CustomerNotes { get; init; }
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateBookingCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(_user.Id);

        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.IsActive, cancellationToken)
            ?? throw new SlotNotAvailableException("Service not found.");

        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.Id == request.BranchId && b.IsActive, cancellationToken)
            ?? throw new SlotNotAvailableException("Branch not found.");

        if (branch.CenterId != service.CenterId)
            throw new SlotNotAvailableException("Service is not offered at this branch.");

        if (request.StaffId is int staffId)
        {
            var staffOk = await _context.Staff
                .AnyAsync(s => s.Id == staffId && s.BranchId == request.BranchId && s.IsActive, cancellationToken);
            if (!staffOk)
                throw new SlotNotAvailableException("Staff member is not valid for this branch.");
        }

        var dow = (int)request.BookingDate.DayOfWeek;
        var daySchedule = await _context.WorkingHours
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.BranchId == request.BranchId && w.DayOfWeek == dow, cancellationToken);

        var existing = await _context.Bookings
            .Where(b => b.BranchId == request.BranchId && b.BookingDate == request.BookingDate)
            .ToListAsync(cancellationToken);

        var timeOffs = await _context.TimeOffs
            .AsNoTracking()
            .Where(t => t.BranchId == request.BranchId
                        && t.FromDate <= request.BookingDate
                        && t.ToDate >= request.BookingDate)
            .ToListAsync(cancellationToken);

        BookingScheduleRules.ValidateNewBooking(
            service,
            branch,
            daySchedule,
            existing,
            timeOffs,
            request.BookingDate,
            request.StartTime,
            request.StaffId);

        var endTime = BookingScheduleRules.AddMinutes(request.StartTime, service.DurationMinutes);

        var booking = new Booking
        {
            CustomerId = _user.Id,
            CenterId = service.CenterId,
            BranchId = request.BranchId,
            ServiceId = service.Id,
            StaffId = request.StaffId,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            EndTime = endTime,
            Status = BookingStatus.Pending,
            CustomerNotes = request.CustomerNotes?.Trim(),
            ServicePrice = service.Price,
            TotalAmount = service.Price
        };

        booking.AddDomainEvent(new BookingCreatedEvent(booking));

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}
