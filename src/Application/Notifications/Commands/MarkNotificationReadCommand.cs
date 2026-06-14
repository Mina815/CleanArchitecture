namespace CleanArchitecture.Application.Notifications.Commands;

public record MarkNotificationReadCommand(int Id) : IRequest;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public MarkNotificationReadCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, notification);

        if (notification!.UserId != _user.Id)
            throw new ForbiddenAccessException();

        notification.IsRead = true;
        notification.ReadAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
