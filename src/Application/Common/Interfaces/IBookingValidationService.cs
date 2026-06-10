namespace CleanArchitecture.Application.Common.Interfaces;

public interface IBookingValidationService
{
    Task ValidateBookingAsync(int branchId, int serviceId, DateOnly bookingDate, TimeOnly startTime, int? staffId, int? excludeBookingId = null, CancellationToken cancellationToken = default);
}
