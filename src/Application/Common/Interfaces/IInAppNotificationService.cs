namespace CleanArchitecture.Application.Common.Interfaces;

public interface IInAppNotificationService
{
    Task CreateAsync(
        string userId,
        string type,
        string title,
        string message,
        string? actionUrl = null,
        string? data = null,
        CancellationToken cancellationToken = default);
}
