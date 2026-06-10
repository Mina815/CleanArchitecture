namespace CleanArchitecture.Application.Common.Interfaces;

public interface IBookingHubService
{
    Task NotifyNewBookingAsync(int branchId, object bookingData, CancellationToken cancellationToken = default);
    Task NotifyBookingConfirmedAsync(int branchId, int bookingId, CancellationToken cancellationToken = default);
    Task NotifyBookingCancelledAsync(int branchId, int bookingId, string? reason, CancellationToken cancellationToken = default);
    Task NotifyBookingCompletedAsync(int branchId, int bookingId, CancellationToken cancellationToken = default);
}
