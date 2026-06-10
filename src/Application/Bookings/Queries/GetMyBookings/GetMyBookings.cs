using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;

namespace CleanArchitecture.Application.Bookings.Queries.GetMyBookings;

[Authorize(Roles = Roles.Customer)]
public record GetMyBookingsQuery : IRequest<List<BookingDto>>;

public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, List<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUser _user;

    public GetMyBookingsQueryHandler(IBookingRepository bookingRepository, IUser user)
    {
        _bookingRepository = bookingRepository;
        _user = user;
    }

    public async Task<List<BookingDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetCustomerBookingsAsync(_user.Id!, cancellationToken);
        return bookings.Select(BookingDto.FromEntity).ToList();
    }
}
