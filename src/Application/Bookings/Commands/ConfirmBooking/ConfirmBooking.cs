using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Commands.ConfirmBooking;

[Authorize(Roles = Roles.Provider)]
public record ConfirmBookingCommand(int BookingId) : IRequest;

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
        Guard.Against.NullOrWhiteSpace(_user.Id);

        var booking = await _context.Bookings
            .Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken)
            ?? throw new CleanArchitecture.Application.Common.Exceptions.NotFoundException("Booking not found.");

        if (booking.Center.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        if (booking.Status != BookingStatus.Pending)
            throw new CleanArchitecture.Application.Common.Exceptions.ValidationException("Only pending bookings can be confirmed.");

        booking.Status = BookingStatus.Confirmed;
        booking.ConfirmedAt = DateTimeOffset.UtcNow;
        booking.AddDomainEvent(new BookingConfirmedEvent(booking));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
