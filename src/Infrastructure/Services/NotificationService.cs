using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure.Services;

public class NotificationService : INotificationService
{
    public Task SendPushNotificationAsync(string userId, string title, string body, object? data = null)
    {
        // TODO: Integrate Firebase Cloud Messaging (FCM) for push notifications
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        // TODO: Integrate SMTP or transactional email service (e.g. SendGrid, MailKit)
        return Task.CompletedTask;
    }
}
