namespace CleanArchitecture.Application.Notifications.Commands;

public record MarkAllNotificationsReadCommand : IRequest;

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public MarkAllNotificationsReadCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        await _context.Notifications
            .Where(n => n.UserId == _user.Id && !n.IsRead)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, now), cancellationToken);
    }
}
