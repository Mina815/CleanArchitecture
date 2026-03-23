using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.Common.Time;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Commands.CancelBooking;

[Authorize(Roles = $"{Roles.Customer},{Roles.Provider}")]
public record CancelBookingCommand(int BookingId, string? Reason = null) : IRequest;

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
        Guard.Against.NullOrWhiteSpace(_user.Id);

        var booking = await _context.Bookings
            .Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken)
            ?? throw new CleanArchitecture.Application.Common.Exceptions.NotFoundException("Booking not found.");

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed)
            throw new CleanArchitecture.Application.Common.Exceptions.ValidationException("Booking cannot be cancelled.");

        if (booking.CustomerId == _user.Id)
        {
            if (_user.Roles?.Contains(Roles.Customer) != true)
                throw new ForbiddenAccessException();

            var appointmentStart = EgyptTime.ToDateTimeOffset(booking.BookingDate, booking.StartTime);
            var hoursUntil = (appointmentStart - DateTimeOffset.UtcNow).TotalHours;
            if (hoursUntil < JamalekConstants.MinimumCancellationHours)
                throw new CancellationNotAllowedException(
                    $"Cancellations must be at least {JamalekConstants.MinimumCancellationHours} hours before the appointment.");
        }
        else if (booking.Center.OwnerId == _user.Id)
        {
            if (_user.Roles?.Contains(Roles.Provider) != true)
                throw new ForbiddenAccessException();
        }
        else
        {
            throw new ForbiddenAccessException();
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTimeOffset.UtcNow;
        booking.CancellationReason = request.Reason?.Trim();
        booking.AddDomainEvent(new BookingCancelledEvent(booking));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
