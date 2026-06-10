using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Notifications.Queries.GetNotifications;

[Authorize]
public record GetNotificationsQuery(bool UnreadOnly = false) : IRequest<List<NotificationDto>>;

public class NotificationDto
{
    public int Id { get; init; }
    public string Type { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Message { get; init; } = null!;
    public string? ActionUrl { get; init; }
    public bool IsRead { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetNotificationsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Notifications.AsNoTracking().Where(n => n.UserId == _user.Id);
        if (request.UnreadOnly) query = query.Where(n => !n.IsRead);

        return await query.OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id, Type = n.Type, Title = n.Title, Message = n.Message,
                ActionUrl = n.ActionUrl, IsRead = n.IsRead, CreatedAt = n.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
