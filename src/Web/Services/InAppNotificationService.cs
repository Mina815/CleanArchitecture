using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Web.Services;

public sealed class InAppNotificationService : IInAppNotificationService
{
    private readonly IApplicationDbContext _context;
    private readonly TimeProvider _timeProvider;

    public InAppNotificationService(IApplicationDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task CreateAsync(
        string userId,
        string type,
        string title,
        string message,
        string? actionUrl = null,
        string? data = null,
        CancellationToken cancellationToken = default)
    {
        _context.Notifications.Add(new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ActionUrl = actionUrl,
            Data = data,
            IsRead = false,
            Created = _timeProvider.GetUtcNow()
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
