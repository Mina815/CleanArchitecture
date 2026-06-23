namespace CleanArchitecture.Application.Bookings.Queries.GetBranchBookings;

public record GetBranchBookingsQuery : IRequest<List<BookingDto>>
{
    public int BranchId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public string? Status { get; init; }
    public bool SortDesc { get; init; } = true;
}

public class GetBranchBookingsQueryHandler : IRequestHandler<GetBranchBookingsQuery, List<BookingDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBranchBookingsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BookingDto>> Handle(GetBranchBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Bookings
            .Where(b => b.BranchId == request.BranchId)
            .AsNoTracking();

        if (request.DateFrom.HasValue)
            query = query.Where(b => b.BookingDate >= DateOnly.FromDateTime(request.DateFrom.Value.Date));

        if (request.DateTo.HasValue)
            query = query.Where(b => b.BookingDate <= DateOnly.FromDateTime(request.DateTo.Value.Date));

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<BookingStatus>(request.Status, out var status))
            query = query.Where(b => b.Status == status);

        IOrderedQueryable<Domain.Entities.Booking> ordered;
        if (request.SortDesc)
            ordered = query.OrderByDescending(b => b.Created)
                           .ThenByDescending(b => b.BookingDate);
        else
            ordered = query.OrderBy(b => b.Created)
                           .ThenBy(b => b.BookingDate);

        return await ordered
            .ProjectTo<BookingDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
