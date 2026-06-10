namespace CleanArchitecture.Domain.Events;

public class BookingCompletedEvent : BaseEvent
{
    public BookingCompletedEvent(int bookingId, string customerId, int centerId, int branchId)
    {
        BookingId = bookingId;
        CustomerId = customerId;
        CenterId = centerId;
        BranchId = branchId;
    }

    public int BookingId { get; }
    public string CustomerId { get; }
    public int CenterId { get; }
    public int BranchId { get; }
}
