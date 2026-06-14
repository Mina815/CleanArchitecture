using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Branches.Commands.UpdateBranch;

[Authorize(Roles = "Provider")]
public record UpdateBranchCommand : IRequest
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? NameAr { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? District { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Phone { get; init; }
    public string? WhatsappNumber { get; init; }
    public bool? IsActive { get; init; }
}

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public UpdateBranchCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Branches
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        var center = await _context.BeautyCenters
            .FindAsync([entity.CenterId], cancellationToken);

        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        if (request.Name is not null) entity.Name = request.Name;
        if (request.NameAr is not null) entity.NameAr = request.NameAr;
        if (request.Address is not null) entity.Address = request.Address;
        if (request.City is not null) entity.City = request.City;
        if (request.Phone is not null) entity.Phone = request.Phone;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
