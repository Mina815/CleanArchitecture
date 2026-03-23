namespace CleanArchitecture.Domain.Entities;

public class Branch : BaseAuditableEntity
{
    public int CenterId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NameAr { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string? WhatsappNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public BeautyCenter Center { get; set; } = null!;

    public IList<Staff> Staff { get; private set; } = new List<Staff>();

    public IList<WorkingHour> WorkingHours { get; private set; } = new List<WorkingHour>();

    public IList<TimeOff> TimeOffs { get; private set; } = new List<TimeOff>();

    public IList<Booking> Bookings { get; private set; } = new List<Booking>();
}
