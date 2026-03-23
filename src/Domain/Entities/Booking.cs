namespace CleanArchitecture.Domain.Entities;

public class Booking : BaseAuditableEntity
{
    public string CustomerId { get; set; } = string.Empty;

    public int CenterId { get; set; }

    public int BranchId { get; set; }

    public int ServiceId { get; set; }

    public int? StaffId { get; set; }

    public DateOnly BookingDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string? CustomerNotes { get; set; }

    public string? CancellationReason { get; set; }

    public DateTimeOffset? ConfirmedAt { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public decimal ServicePrice { get; set; }

    public decimal TotalAmount { get; set; }

    public BeautyCenter Center { get; set; } = null!;

    public Branch Branch { get; set; } = null!;

    public Service Service { get; set; } = null!;

    public Staff? Staff { get; set; }

    public Payment? Payment { get; set; }

    public Review? Review { get; set; }
}
