using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Commands.ConfirmBooking;

[Authorize(Roles = Roles.Provider)]
public record ConfirmBookingCommand(int Id) : IRequest;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public ConfirmBookingCommandHandler(IBookingRepository bookingRepository, IApplicationDbContext context, IUser user)
    {
        _bookingRepository = bookingRepository;
        _context = context;
        _user = user;
    }

    public async Task Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, booking);

        var ownsCenter = await _context.BeautyCenters
            .AnyAsync(c => c.Id == booking.CenterId && c.OwnerId == _user.Id, cancellationToken);
        if (!ownsCenter) throw new ForbiddenAccessException();

        booking.Confirm();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
