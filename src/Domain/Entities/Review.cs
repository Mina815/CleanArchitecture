namespace CleanArchitecture.Domain.Entities;

public class Review : BaseAuditableEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public int CenterId { get; set; }
    public int BookingId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
}
