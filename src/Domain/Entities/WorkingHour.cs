namespace CleanArchitecture.Domain.Entities;

public class WorkingHour : BaseEntity
{
    public int BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }

    public bool IsClosed { get; set; }
}
