namespace CleanArchitecture.Application.Branches.Queries.GetCenterBranches;

public class BranchDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public bool IsActive { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Branch, BranchDto>();
        }
    }
}
