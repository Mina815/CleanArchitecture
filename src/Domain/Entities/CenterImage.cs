namespace CleanArchitecture.Domain.Entities;

public class CenterImage : BaseEntity
{
    public int CenterId { get; set; }

    public BeautyCenter Center { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }
}
