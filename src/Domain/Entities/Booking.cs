namespace CleanArchitecture.Domain.Entities;

public class Booking : BaseAuditableEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public int CenterId { get; set; }
    public int BranchId { get; set; }
    public int ServiceId { get; set; }
    public int? StaffId { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? CustomerNotes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public decimal ServicePrice { get; set; }
    public decimal TotalAmount { get; set; }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            return;

        Status = BookingStatus.Confirmed;
        ConfirmedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new BookingConfirmedEvent(this));
    }

    public void Cancel(string? reason = null)
    {
        if (Status is BookingStatus.Cancelled or BookingStatus.Completed)
            return;

        Status = BookingStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
        CancellationReason = reason;
        AddDomainEvent(new BookingCancelledEvent(this));
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            return;

        Status = BookingStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new BookingCompletedEvent(this));
    }
}
