namespace CleanArchitecture.Domain.Entities;

public class Staff : BaseAuditableEntity
{
    public int BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? ImageUrl { get; set; }

    public string? Specialization { get; set; }

    public bool IsActive { get; set; } = true;

    public IList<TimeOff> TimeOffs { get; private set; } = new List<TimeOff>();

    public IList<Booking> Bookings { get; private set; } = new List<Booking>();
}
