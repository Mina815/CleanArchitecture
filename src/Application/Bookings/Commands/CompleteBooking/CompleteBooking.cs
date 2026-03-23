using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Commands.CompleteBooking;

[Authorize(Roles = Roles.Provider)]
public record CompleteBookingCommand(int BookingId) : IRequest;

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
        Guard.Against.NullOrWhiteSpace(_user.Id);

        var booking = await _context.Bookings
            .Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken)
            ?? throw new CleanArchitecture.Application.Common.Exceptions.NotFoundException("Booking not found.");

        if (booking.Center.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        if (booking.Status != BookingStatus.Confirmed)
            throw new CleanArchitecture.Application.Common.Exceptions.ValidationException("Only confirmed bookings can be completed.");

        booking.Status = BookingStatus.Completed;
        booking.CompletedAt = DateTimeOffset.UtcNow;
        booking.AddDomainEvent(new BookingCompletedEvent(booking));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
