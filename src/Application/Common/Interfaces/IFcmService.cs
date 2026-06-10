namespace CleanArchitecture.Application.Common.Interfaces;

public interface IFcmService
{
    Task SendAsync(string? fcmToken, string title, string message, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}
