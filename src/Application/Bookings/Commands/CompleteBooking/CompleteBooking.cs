using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Commands.CompleteBooking;

[Authorize(Roles = Roles.Provider)]
public record CompleteBookingCommand(int Id) : IRequest;

public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CompleteBookingCommandHandler(IBookingRepository bookingRepository, IApplicationDbContext context, IUser user)
    {
        _bookingRepository = bookingRepository;
        _context = context;
        _user = user;
    }

    public async Task Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, booking);

        var ownsCenter = await _context.BeautyCenters
            .AnyAsync(c => c.Id == booking.CenterId && c.OwnerId == _user.Id, cancellationToken);
        if (!ownsCenter) throw new ForbiddenAccessException();

        booking.Complete();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
