namespace CleanArchitecture.Domain.Events;

public class BookingConfirmedEvent : BaseEvent
{
    public BookingConfirmedEvent(int bookingId, string customerId, int branchId, int centerId)
    {
        BookingId = bookingId;
        CustomerId = customerId;
        BranchId = branchId;
        CenterId = centerId;
    }

    public int BookingId { get; }
    public string CustomerId { get; }
    public int BranchId { get; }
    public int CenterId { get; }
}
