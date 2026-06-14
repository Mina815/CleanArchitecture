using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Services.Commands.CreateService;

[Authorize(Roles = "Provider")]
public record CreateServiceCommand : IRequest<int>
{
    public int CenterId { get; init; }
    public int CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
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
            .FindAsync([request.CenterId], cancellationToken);

        Guard.Against.NotFound(request.CenterId, center);

        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        var entity = new Domain.Entities.Service
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
            DisplayOrder = request.DisplayOrder
        };

        _context.Services.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
