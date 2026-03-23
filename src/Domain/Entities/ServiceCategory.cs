namespace CleanArchitecture.Domain.Entities;

public class ServiceCategory : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string NameAr { get; set; } = string.Empty;

    public string? IconUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public IList<Service> Services { get; private set; } = new List<Service>();
}
