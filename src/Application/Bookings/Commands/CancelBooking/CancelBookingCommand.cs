namespace CleanArchitecture.Application.Bookings.Commands.CancelBooking;

public record CancelBookingCommand : IRequest
{
    public int Id { get; init; }
    public string? Reason { get; init; }
}

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CancelBookingCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FindAsync([request.Id], cancellationToken);
        Guard.Against.NotFound(request.Id, booking);

        var isCustomer = _user.Roles?.Contains(Roles.Customer) ?? false;
        var isProvider = _user.Roles?.Contains(Roles.Provider) ?? false;

        if (isCustomer)
        {
            if (booking!.CustomerId != _user.Id)
                throw new ForbiddenAccessException();

            var appointmentTime = booking.BookingDate.ToDateTime(TimeOnly.FromTimeSpan(booking.StartTime), DateTimeKind.Utc);
            var hoursUntilAppointment = (appointmentTime - DateTimeOffset.UtcNow).TotalHours;

            if (hoursUntilAppointment < 24)
                throw new CancellationNotAllowedException(
                    "Bookings can only be cancelled at least 24 hours before the appointment.");
        }
        else if (isProvider)
        {
            var center = await _context.BeautyCenters
                .FindAsync([booking!.CenterId], cancellationToken);

            if (center!.OwnerId != _user.Id)
                throw new ForbiddenAccessException();
        }
        else
        {
            throw new ForbiddenAccessException();
        }

        booking!.Cancel(request.Reason);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
