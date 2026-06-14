namespace CleanArchitecture.Application.Bookings.Queries.GetBranchBookingsToday;

public record GetBranchBookingsTodayQuery(int BranchId) : IRequest<List<BookingDto>>;

public class GetBranchBookingsTodayQueryHandler : IRequestHandler<GetBranchBookingsTodayQuery, List<BookingDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBranchBookingsTodayQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BookingDto>> Handle(GetBranchBookingsTodayQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _context.Bookings
            .Where(b => b.BranchId == request.BranchId
                     && b.BookingDate == today
                     && b.Status != BookingStatus.Cancelled)
            .AsNoTracking()
            .OrderBy(b => b.StartTime)
            .ProjectTo<BookingDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
