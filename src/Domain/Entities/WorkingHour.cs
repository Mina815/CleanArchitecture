namespace CleanArchitecture.Domain.Entities;

public class WorkingHour : BaseEntity
{
    public int BranchId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
    public bool IsClosed { get; set; }
}
