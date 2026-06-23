namespace CleanArchitecture.Application.Bookings.Queries.GetAvailableSlots;

public record GetAvailableSlotsQuery : IRequest<List<TimeSlotDto>>
{
    public int BranchId { get; init; }
    public int ServiceId { get; init; }
    public DateTime Date { get; init; }
    public int? StaffId { get; init; }
}

public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, List<TimeSlotDto>>
{
    private readonly IApplicationDbContext _context;

    private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);

    public GetAvailableSlotsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TimeSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var bookingDate = DateOnly.FromDateTime(request.Date.Date);

        var service = await _context.Services
            .FindAsync([request.ServiceId], cancellationToken);
        Guard.Against.NotFound(request.ServiceId, service);

        var dayOfWeek = bookingDate.DayOfWeek;
        var workingHour = await _context.WorkingHours
            .FirstOrDefaultAsync(w => w.BranchId == request.BranchId
                                   && w.DayOfWeek == dayOfWeek, cancellationToken);

        if (workingHour is null || workingHour.IsClosed)
            return new List<TimeSlotDto>();

        var existingBookings = await _context.Bookings
            .Where(b => b.BranchId == request.BranchId
                     && b.BookingDate == bookingDate
                     && b.Status != BookingStatus.Cancelled
                     && b.Status != BookingStatus.Completed)
            .ToListAsync(cancellationToken);

        var timeOffs = await _context.TimeOffs
            .Where(t => t.BranchId == request.BranchId
                     && t.FromDate <= bookingDate
                     && t.ToDate >= bookingDate)
            .ToListAsync(cancellationToken);

        if (request.StaffId.HasValue)
        {
            timeOffs = timeOffs.Where(t => !t.StaffId.HasValue || t.StaffId == request.StaffId).ToList();
        }

        var slots = new List<TimeSlotDto>();
        var slotStart = workingHour.OpenTime;
        var serviceDuration = TimeSpan.FromMinutes(service!.DurationMinutes);

        while (slotStart.Add(serviceDuration) <= workingHour.CloseTime)
        {
            var slotEnd = slotStart.Add(serviceDuration);
            var isAvailable = true;

            foreach (var booking in existingBookings)
            {
                if (booking.StartTime < slotEnd && booking.EndTime > slotStart)
                {
                    isAvailable = false;
                    break;
                }
            }

            if (isAvailable)
            {
                foreach (var timeOff in timeOffs)
                {
                    var offFrom = timeOff.FromTime ?? workingHour.OpenTime;
                    var offTo = timeOff.ToTime ?? workingHour.CloseTime;

                    if (offFrom < slotEnd && offTo > slotStart)
                    {
                        isAvailable = false;
                        break;
                    }
                }
            }

            slots.Add(new TimeSlotDto
            {
                StartTime = slotStart,
                EndTime = slotEnd,
                IsAvailable = isAvailable
            });

            slotStart = slotStart.Add(SlotDuration);
        }

        return slots;
    }
}
