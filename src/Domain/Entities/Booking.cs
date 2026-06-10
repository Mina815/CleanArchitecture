using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.Entities;

public class Booking : BaseAuditableEntity
{
    public string CustomerId { get; set; } = null!;

    public int CenterId { get; set; }

    public BeautyCenter Center { get; set; } = null!;

    public int BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public int ServiceId { get; set; }

    public CenterService Service { get; set; } = null!;

    public int? StaffId { get; set; }

    public Staff? Staff { get; set; }

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

    public Payment? Payment { get; set; }

    public Review? Review { get; set; }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed.");

        Status = BookingStatus.Confirmed;
        ConfirmedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new BookingConfirmedEvent(Id, CustomerId, BranchId, CenterId));
    }

    public void Cancel(string? reason, bool isProvider)
    {
        if (Status is BookingStatus.Cancelled or BookingStatus.Completed)
            throw new InvalidOperationException("Booking cannot be cancelled.");

        if (!isProvider)
        {
            var appointmentDateTime = BookingDate.ToDateTime(StartTime);
            var hoursUntil = (appointmentDateTime - DateTime.Now).TotalHours;
            if (hoursUntil < 24)
                throw new CancellationNotAllowedException();
        }

        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new BookingCancelledEvent(Id, CustomerId, BranchId, CenterId, reason));
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be completed.");

        Status = BookingStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new BookingCompletedEvent(Id, CustomerId, CenterId, BranchId));
    }

    public static Booking Create(
        string customerId,
        int centerId,
        int branchId,
        int serviceId,
        int? staffId,
        DateOnly bookingDate,
        TimeOnly startTime,
        TimeOnly endTime,
        decimal servicePrice,
        string? customerNotes)
    {
        var booking = new Booking
        {
            CustomerId = customerId,
            CenterId = centerId,
            BranchId = branchId,
            ServiceId = serviceId,
            StaffId = staffId,
            BookingDate = bookingDate,
            StartTime = startTime,
            EndTime = endTime,
            ServicePrice = servicePrice,
            TotalAmount = servicePrice,
            CustomerNotes = customerNotes,
            Status = BookingStatus.Pending
        };

        return booking;
    }

    public void RaiseCreatedEvent()
    {
        AddDomainEvent(new BookingCreatedEvent(Id, CustomerId, BranchId, CenterId, BookingDate, StartTime));
    }
}
