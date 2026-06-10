using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Staff;

public class StaffDto
{
    public int Id { get; init; }
    public int BranchId { get; init; }
    public string Name { get; init; } = null!;
    public string? Phone { get; init; }
    public string? ImageUrl { get; init; }
    public string? Specialization { get; init; }
    public bool IsActive { get; init; }

    public static StaffDto FromEntity(Domain.Entities.Staff s) => new()
    {
        Id = s.Id,
        BranchId = s.BranchId,
        Name = s.Name,
        Phone = s.Phone,
        ImageUrl = s.ImageUrl,
        Specialization = s.Specialization,
        IsActive = s.IsActive
    };
}
