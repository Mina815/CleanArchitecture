using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Entities;

public class Booking : BaseAuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid CenterId { get; set; }
    public Guid BranchId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid? StaffId { get; set; }

    public DateOnly BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? CustomerNotes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public decimal ServicePrice { get; set; }
    public decimal TotalAmount { get; set; }

    // Navigation Properties
    //public User Customer { get; set; } = null!;
    public BeautyCenter Center { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Staff? Staff { get; set; }
    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
}
