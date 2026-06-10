namespace CleanArchitecture.Application.Common.Interfaces;

public record TimeSlotDto(TimeOnly StartTime, TimeOnly EndTime, bool IsAvailable);

public interface IAvailabilityService
{
    Task<List<TimeSlotDto>> GetAvailableSlotsAsync(int branchId, int serviceId, DateOnly date, int? staffId = null, CancellationToken cancellationToken = default);
}
