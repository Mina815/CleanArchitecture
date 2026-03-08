using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Entities;

public class Payment : BaseAuditableEntity
{
    public Guid BookingId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public PaymentProvider Provider { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod Method { get; set; }

    public string? PaymentUrl { get; set; }
    public string? ProviderReference { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? FailureReason { get; set; }

    // Navigation Properties
    public Booking Booking { get; set; } = null!;
}
