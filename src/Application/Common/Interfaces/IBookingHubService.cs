namespace CleanArchitecture.Application.Common.Interfaces;

public interface IBookingHubService
{
    Task NotifyNewBookingAsync(int branchId, object bookingData);
    Task NotifyBookingConfirmedAsync(int branchId, int bookingId);
    Task NotifyBookingCancelledAsync(int branchId, int bookingId, string? reason);
    Task NotifyBookingCompletedAsync(int branchId, int bookingId);
}
