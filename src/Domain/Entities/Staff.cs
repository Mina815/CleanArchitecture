using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CleanArchitecture.Domain.Entities;

public class Staff : BaseAuditableEntity
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ImageUrl { get; set; }
    public string? Specialization { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Branch Branch { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<TimeOff> TimeOffs { get; set; } = new List<TimeOff>();
}
