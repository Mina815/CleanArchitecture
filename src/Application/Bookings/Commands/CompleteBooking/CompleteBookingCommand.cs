using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Bookings.Commands.CompleteBooking;

[Authorize(Roles = "Provider")]
public record CompleteBookingCommand(int Id) : IRequest;

public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CompleteBookingCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FindAsync([request.Id], cancellationToken);
        Guard.Against.NotFound(request.Id, booking);

        var center = await _context.BeautyCenters
            .FindAsync([booking!.CenterId], cancellationToken);

        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        booking.Complete();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
