namespace CleanArchitecture.Domain.Entities;

public class WorkingHour : BaseAuditableEntity
{
    public int BranchId { get; set; }

    /// <summary>0 = Sunday … 6 = Saturday (same convention as .NET DayOfWeek).</summary>
    public int DayOfWeek { get; set; }

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }

    public bool IsClosed { get; set; }

    public Branch Branch { get; set; } = null!;
}
