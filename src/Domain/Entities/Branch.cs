namespace CleanArchitecture.Domain.Entities;

public class Branch : BaseAuditableEntity
{
    public int CenterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? WhatsappNumber { get; set; }
    public bool IsActive { get; set; } = true;
}
