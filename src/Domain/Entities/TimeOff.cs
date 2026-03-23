namespace CleanArchitecture.Domain.Entities;

public class TimeOff : BaseAuditableEntity
{
    public int BranchId { get; set; }

    public int? StaffId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public TimeOnly? FromTime { get; set; }

    public TimeOnly? ToTime { get; set; }

    public string? Reason { get; set; }

    public TimeOffType Type { get; set; }

    public Branch Branch { get; set; } = null!;

    public Staff? Staff { get; set; }
}
