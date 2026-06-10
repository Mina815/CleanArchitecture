using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Centers.Commands.UpdateCenter;

[Authorize(Roles = Roles.Provider)]
public record UpdateCenterCommand : IRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
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
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.OwnerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.Description = request.Description;
        entity.DescriptionAr = request.DescriptionAr;
        entity.LogoUrl = request.LogoUrl;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
