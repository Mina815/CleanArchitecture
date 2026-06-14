using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Branches.Commands.CreateBranch;

[Authorize(Roles = "Provider")]
public record CreateBranchCommand : IRequest<int>
{
    public int CenterId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string? District { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Phone { get; init; }
    public string? WhatsappNumber { get; init; }
}

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateBranchCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var center = await _context.BeautyCenters
            .FindAsync([request.CenterId], cancellationToken);

        Guard.Against.NotFound(request.CenterId, center);

        if (center.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        var entity = new Branch
        {
            CenterId = request.CenterId,
            Name = request.Name,
            NameAr = request.NameAr,
            Address = request.Address,
            City = request.City,
            District = request.District,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Phone = request.Phone,
            WhatsappNumber = request.WhatsappNumber
        };

        _context.Branches.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
