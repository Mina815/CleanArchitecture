namespace CleanArchitecture.Domain.Entities;

public class CenterService : BaseAuditableEntity
{
    public int CenterId { get; set; }

    public BeautyCenter Center { get; set; } = null!;

    public int CategoryId { get; set; }

    public ServiceCategory Category { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public string? Description { get; set; }

    public string? DescriptionAr { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; } = 30;

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }

    public IList<Booking> Bookings { get; private set; } = new List<Booking>();
}
