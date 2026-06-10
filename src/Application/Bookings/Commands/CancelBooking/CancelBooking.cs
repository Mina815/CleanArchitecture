using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Commands.CancelBooking;

[Authorize]
public record CancelBookingCommand(int Id, string? Reason) : IRequest;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CancelBookingCommandHandler(IBookingRepository bookingRepository, IApplicationDbContext context, IUser user)
    {
        _bookingRepository = bookingRepository;
        _context = context;
        _user = user;
    }

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, booking);

        var isProvider = _user.Roles?.Contains(Roles.Provider) == true;
        var isOwner = booking.CustomerId == _user.Id;
        var ownsCenter = isProvider && await _context.BeautyCenters
            .AnyAsync(c => c.Id == booking.CenterId && c.OwnerId == _user.Id, cancellationToken);

        if (!isOwner && !ownsCenter) throw new ForbiddenAccessException();

        booking.Cancel(request.Reason, ownsCenter);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
