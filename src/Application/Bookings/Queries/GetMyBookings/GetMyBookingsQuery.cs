namespace CleanArchitecture.Application.Bookings.Queries.GetMyBookings;

public record GetMyBookingsQuery : IRequest<List<BookingDto>>
{
    public bool? Upcoming { get; init; }
}

public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, List<BookingDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _user;

    public GetMyBookingsQueryHandler(IApplicationDbContext context, IMapper mapper, IUser user)
    {
        _context = context;
        _mapper = mapper;
        _user = user;
    }

    public async Task<List<BookingDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Bookings
            .Where(b => b.CustomerId == _user.Id)
            .AsNoTracking();

        if (request.Upcoming.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            if (request.Upcoming.Value)
                query = query.Where(b => b.BookingDate >= today
                                      && b.Status != BookingStatus.Cancelled
                                      && b.Status != BookingStatus.Completed);
            else
                query = query.Where(b => b.BookingDate < today
                                      || b.Status == BookingStatus.Cancelled
                                      || b.Status == BookingStatus.Completed);
        }

        return await query
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.StartTime)
            .ProjectTo<BookingDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
