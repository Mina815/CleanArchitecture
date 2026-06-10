namespace CleanArchitecture.Domain.Entities;

public class ServiceCategory : BaseAuditableEntity
{
    public string Name { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public string? IconUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public IList<CenterService> Services { get; private set; } = new List<CenterService>();
}
