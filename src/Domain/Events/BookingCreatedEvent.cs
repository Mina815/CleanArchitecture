namespace CleanArchitecture.Domain.Events;

public class BookingCreatedEvent : BaseEvent
{
    public BookingCreatedEvent(int bookingId, string customerId, int branchId, int centerId, DateOnly bookingDate, TimeOnly startTime)
    {
        BookingId = bookingId;
        CustomerId = customerId;
        BranchId = branchId;
        CenterId = centerId;
        BookingDate = bookingDate;
        StartTime = startTime;
    }

    public int BookingId { get; }
    public string CustomerId { get; }
    public int BranchId { get; }
    public int CenterId { get; }
    public DateOnly BookingDate { get; }
    public TimeOnly StartTime { get; }
}
