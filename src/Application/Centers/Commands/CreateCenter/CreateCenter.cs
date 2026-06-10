using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Centers.Commands.CreateCenter;

[Authorize(Roles = Roles.Provider)]
public record CreateCenterCommand : IRequest<int>
{
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public string? LogoUrl { get; init; }
}

public class CreateCenterCommandHandler : IRequestHandler<CreateCenterCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateCenterCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateCenterCommand request, CancellationToken cancellationToken)
    {
        var entity = new BeautyCenter
        {
            OwnerId = _user.Id!,
            Name = request.Name,
            NameAr = request.NameAr,
            Description = request.Description,
            DescriptionAr = request.DescriptionAr,
            LogoUrl = request.LogoUrl,
            IsActive = true,
            IsVerified = false
        };

        _context.BeautyCenters.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
