namespace CleanArchitecture.Application.Bookings.Queries.GetBookingById;

public record GetBookingByIdQuery(int Id) : IRequest<BookingDetailDto>;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBookingByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BookingDetailDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .ProjectTo<BookingDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, booking);

        return booking;
    }
}
