namespace CleanArchitecture.Domain.Entities;

public class Service : BaseAuditableEntity
{
    public int CenterId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NameAr { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? DescriptionAr { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }

    public BeautyCenter Center { get; set; } = null!;

    public ServiceCategory Category { get; set; } = null!;

    public IList<Booking> Bookings { get; private set; } = new List<Booking>();
}
