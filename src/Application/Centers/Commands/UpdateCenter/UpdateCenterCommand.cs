using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Centers.Commands.UpdateCenter;

[Authorize(Roles = "Provider")]
public record UpdateCenterCommand : IRequest
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? NameAr { get; init; }
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public string? LogoUrl { get; init; }
}

public class UpdateCenterCommandHandler : IRequestHandler<UpdateCenterCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public UpdateCenterCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(UpdateCenterCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BeautyCenters
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (entity.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        if (request.Name is not null)
            entity.Name = request.Name;

        if (request.NameAr is not null)
            entity.NameAr = request.NameAr;

        if (request.Description is not null)
            entity.Description = request.Description;

        if (request.DescriptionAr is not null)
            entity.DescriptionAr = request.DescriptionAr;

        if (request.LogoUrl is not null)
            entity.LogoUrl = request.LogoUrl;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
