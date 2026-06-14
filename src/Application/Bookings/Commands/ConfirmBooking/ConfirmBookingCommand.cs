using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Bookings.Commands.ConfirmBooking;

[Authorize(Roles = "Provider")]
public record ConfirmBookingCommand(int Id) : IRequest;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public ConfirmBookingCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FindAsync([request.Id], cancellationToken);
        Guard.Against.NotFound(request.Id, booking);

        var center = await _context.BeautyCenters
            .FindAsync([booking!.CenterId], cancellationToken);

        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        booking.Confirm();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
