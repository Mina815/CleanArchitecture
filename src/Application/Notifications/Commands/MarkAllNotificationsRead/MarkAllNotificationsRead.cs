using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Notifications.Commands.MarkAllNotificationsRead;

[Authorize]
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
        var notifications = await _context.Notifications
            .Where(n => n.UserId == _user.Id && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var n in notifications)
        {
            n.IsRead = true;
            n.ReadAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
