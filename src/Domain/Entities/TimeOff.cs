namespace CleanArchitecture.Domain.Entities;

public class TimeOff : BaseEntity
{
    public int BranchId { get; set; }
    public int? StaffId { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public TimeSpan? FromTime { get; set; }
    public TimeSpan? ToTime { get; set; }
    public string? Reason { get; set; }
    public TimeOffType Type { get; set; }
}
