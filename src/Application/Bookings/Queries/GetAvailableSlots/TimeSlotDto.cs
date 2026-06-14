namespace CleanArchitecture.Application.Bookings.Queries.GetAvailableSlots;

public class TimeSlotDto
{
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public bool IsAvailable { get; init; }
}
