namespace CleanArchitecture.Domain.Entities;

public class Branch : BaseAuditableEntity
{
    public int CenterId { get; set; }

    public BeautyCenter Center { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? District { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string Phone { get; set; } = null!;

    public string? WhatsappNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public IList<Staff> StaffMembers { get; private set; } = new List<Staff>();

    public IList<WorkingHour> WorkingHours { get; private set; } = new List<WorkingHour>();

    public IList<TimeOff> TimeOffs { get; private set; } = new List<TimeOff>();

    public IList<Booking> Bookings { get; private set; } = new List<Booking>();
}
