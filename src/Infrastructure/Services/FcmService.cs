using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services;

public class FcmService : IFcmService
{
    private readonly ILogger<FcmService> _logger;

    public FcmService(ILogger<FcmService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string? fcmToken, string title, string message, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fcmToken))
            return Task.CompletedTask;

        _logger.LogInformation("FCM push to {Token}: {Title} - {Message}", fcmToken[..Math.Min(8, fcmToken.Length)], title, message);
        return Task.CompletedTask;
    }
}
