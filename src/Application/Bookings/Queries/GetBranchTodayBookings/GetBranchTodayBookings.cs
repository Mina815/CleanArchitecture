using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Queries.GetBranchTodayBookings;

[Authorize(Roles = Roles.Provider)]
public record GetBranchTodayBookingsQuery(int BranchId) : IRequest<List<BookingDto>>;

public class GetBranchTodayBookingsQueryHandler : IRequestHandler<GetBranchTodayBookingsQuery, List<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetBranchTodayBookingsQueryHandler(IBookingRepository bookingRepository, IApplicationDbContext context, IUser user)
    {
        _bookingRepository = bookingRepository;
        _context = context;
        _user = user;
    }

    public async Task<List<BookingDto>> Handle(GetBranchTodayBookingsQuery request, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches.Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);
        Guard.Against.NotFound(request.BranchId, branch);

        if (branch.Center.OwnerId != _user.Id) throw new ForbiddenAccessException();

        var today = DateOnly.FromDateTime(DateTime.Now);
        var bookings = await _bookingRepository.GetByBranchAndDateAsync(request.BranchId, today, cancellationToken);
        return bookings.Select(BookingDto.FromEntity).ToList();
    }
}
