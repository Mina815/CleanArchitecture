namespace CleanArchitecture.Domain.Entities;

public class CenterImage : BaseAuditableEntity
{
    public int CenterId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }

    public BeautyCenter Center { get; set; } = null!;
}
