using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Services.Commands.CreateService;

[Authorize(Roles = Roles.Provider)]
public record CreateServiceCommand : IRequest<int>
{
    public int CenterId { get; init; }
    public int CategoryId { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; } = 30;
    public string? ImageUrl { get; init; }
    public int DisplayOrder { get; init; }
}

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateServiceCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var center = await _context.BeautyCenters
            .FirstOrDefaultAsync(c => c.Id == request.CenterId && c.OwnerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.CenterId, center);

        var category = await _context.ServiceCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.IsActive, cancellationToken);
        Guard.Against.NotFound(request.CategoryId, category);

        var entity = new CenterService
        {
            CenterId = request.CenterId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            NameAr = request.NameAr,
            Description = request.Description,
            DescriptionAr = request.DescriptionAr,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder,
            IsActive = true
        };

        _context.CenterServices.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
