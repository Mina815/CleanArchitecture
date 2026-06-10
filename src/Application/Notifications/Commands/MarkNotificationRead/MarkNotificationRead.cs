using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Notifications.Commands.MarkNotificationRead;

[Authorize]
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
            .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, notification);

        notification.IsRead = true;
        notification.ReadAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
