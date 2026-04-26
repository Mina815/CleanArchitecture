namespace CleanArchitecture.Application.Common.Interfaces;

public interface IBookingRealtimeNotifier
{
    Task NewBookingAsync(int branchId, int bookingId, CancellationToken cancellationToken = default);

    Task BookingConfirmedAsync(int branchId, int bookingId, CancellationToken cancellationToken = default);

    Task BookingCancelledAsync(int branchId, int bookingId, string? reason, CancellationToken cancellationToken = default);

    Task BookingCompletedAsync(int branchId, int bookingId, CancellationToken cancellationToken = default);
}
