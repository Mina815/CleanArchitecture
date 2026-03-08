using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Entities;

public class TimeOff : BaseAuditableEntity
{
    public Guid? BranchId { get; set; }
    public Guid? StaffId { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public TimeOnly? FromTime { get; set; }
    public TimeOnly? ToTime { get; set; }
    public string? Reason { get; set; }
    public TimeOffType Type { get; set; }

    // Navigation Properties
    public Branch? Branch { get; set; }
    public Staff? Staff { get; set; }
}
