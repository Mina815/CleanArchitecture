namespace CleanArchitecture.Application.Notifications.Queries;

public record GetMyNotificationsQuery : IRequest<List<NotificationDto>>
{
    public bool? UnreadOnly { get; init; }
}

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _user;

    public GetMyNotificationsQueryHandler(IApplicationDbContext context, IMapper mapper, IUser user)
    {
        _context = context;
        _mapper = mapper;
        _user = user;
    }

    public async Task<List<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == _user.Id)
            .AsNoTracking();

        if (request.UnreadOnly.HasValue && request.UnreadOnly.Value)
            query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
