namespace CleanArchitecture.Domain.Entities;

public class Payment : BaseAuditableEntity
{
    public int BookingId { get; set; }

    public string? TransactionId { get; set; }

    public PaymentProvider Provider { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "EGP";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? Method { get; set; }

    public string? PaymentUrl { get; set; }

    public string? ProviderReference { get; set; }

    public DateTimeOffset? PaidAt { get; set; }

    public string? FailureReason { get; set; }

    public Booking Booking { get; set; } = null!;
}
