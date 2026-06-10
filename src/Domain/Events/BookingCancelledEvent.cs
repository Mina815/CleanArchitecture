namespace CleanArchitecture.Domain.Events;

public class BookingCancelledEvent : BaseEvent
{
    public BookingCancelledEvent(int bookingId, string customerId, int branchId, int centerId, string? reason)
    {
        BookingId = bookingId;
        CustomerId = customerId;
        BranchId = branchId;
        CenterId = centerId;
        Reason = reason;
    }

    public int BookingId { get; }
    public string CustomerId { get; }
    public int BranchId { get; }
    public int CenterId { get; }
    public string? Reason { get; }
}
