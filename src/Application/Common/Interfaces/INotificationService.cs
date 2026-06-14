namespace CleanArchitecture.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendPushNotificationAsync(string userId, string title, string body, object? data = null);
    Task SendEmailAsync(string to, string subject, string body);
}
