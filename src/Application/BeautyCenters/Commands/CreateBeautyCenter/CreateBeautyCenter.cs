using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.BeautyCenters.Commands.CreateBeautyCenter;

[Authorize(Roles = Roles.Provider)]
public record CreateBeautyCenterCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;

    public string NameAr { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? DescriptionAr { get; init; }

    public string? LogoUrl { get; init; }
}

public class CreateBeautyCenterCommandHandler : IRequestHandler<CreateBeautyCenterCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateBeautyCenterCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateBeautyCenterCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(_user.Id);

        var entity = new BeautyCenter
        {
            OwnerId = _user.Id,
            Name = request.Name.Trim(),
            NameAr = request.NameAr.Trim(),
            Description = request.Description?.Trim(),
            DescriptionAr = request.DescriptionAr?.Trim(),
            LogoUrl = request.LogoUrl?.Trim(),
            IsActive = true,
            IsVerified = false
        };

        _context.BeautyCenters.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
