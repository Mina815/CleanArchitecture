namespace CleanArchitecture.Application.Bookings.Commands.CreateBooking;

public record CreateBookingCommand : IRequest<int>
{
    public int CenterId { get; init; }
    public int BranchId { get; init; }
    public int ServiceId { get; init; }
    public int? StaffId { get; init; }
    public DateOnly BookingDate { get; init; }
    public TimeSpan StartTime { get; init; }
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
        var service = await _context.Services
            .FindAsync([request.ServiceId], cancellationToken);
        Guard.Against.NotFound(request.ServiceId, service);

        var branch = await _context.Branches
            .FindAsync([request.BranchId], cancellationToken);
        Guard.Against.NotFound(request.BranchId, branch);

        var endTime = request.StartTime.Add(TimeSpan.FromMinutes(service!.DurationMinutes));

        var dayOfWeek = request.BookingDate.DayOfWeek;
        var workingHour = await _context.WorkingHours
            .FirstOrDefaultAsync(w => w.BranchId == request.BranchId
                                   && w.DayOfWeek == dayOfWeek, cancellationToken);

        if (workingHour is null || workingHour.IsClosed)
            throw new SlotNotAvailableException("Branch is closed on this day.");

        if (request.StartTime < workingHour.OpenTime || endTime > workingHour.CloseTime)
            throw new SlotNotAvailableException("Booking time is outside working hours.");

        var hasOverlap = await _context.Bookings
            .AnyAsync(b => b.BranchId == request.BranchId
                        && b.BookingDate == request.BookingDate
                        && b.Status != BookingStatus.Cancelled
                        && b.Status != BookingStatus.Completed
                        && b.StartTime < endTime
                        && b.EndTime > request.StartTime, cancellationToken);

        if (hasOverlap)
            throw new SlotNotAvailableException("This time slot is already booked.");

        if (request.StaffId.HasValue)
        {
            var staffTimeOff = await _context.TimeOffs
                .AnyAsync(t => t.StaffId == request.StaffId
                            && t.FromDate <= request.BookingDate
                            && t.ToDate >= request.BookingDate
                            && (!t.FromTime.HasValue || t.FromTime <= request.StartTime)
                            && (!t.ToTime.HasValue || t.ToTime >= endTime), cancellationToken);

            if (staffTimeOff)
                throw new SlotNotAvailableException("Staff member is on time-off during this period.");
        }

        var booking = new Booking
        {
            CustomerId = _user.Id!,
            CenterId = request.CenterId,
            BranchId = request.BranchId,
            ServiceId = request.ServiceId,
            StaffId = request.StaffId,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            EndTime = endTime,
            ServicePrice = service.Price,
            TotalAmount = service.Price,
            CustomerNotes = request.CustomerNotes
        };

        booking.AddDomainEvent(new BookingCreatedEvent(booking));

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}
