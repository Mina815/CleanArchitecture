using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Services.Commands.UpdateService;

[Authorize(Roles = "Provider")]
public record UpdateServiceCommand : IRequest
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? NameAr { get; init; }
    public string? Description { get; init; }
    public string? DescriptionAr { get; init; }
    public decimal? Price { get; init; }
    public int? DurationMinutes { get; init; }
    public string? ImageUrl { get; init; }
    public int? DisplayOrder { get; init; }
    public bool? IsActive { get; init; }
}

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public UpdateServiceCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Services
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        var center = await _context.BeautyCenters
            .FindAsync([entity!.CenterId], cancellationToken);

        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        if (request.Name is not null) entity.Name = request.Name;
        if (request.NameAr is not null) entity.NameAr = request.NameAr;
        if (request.Price.HasValue) entity.Price = request.Price.Value;
        if (request.DurationMinutes.HasValue) entity.DurationMinutes = request.DurationMinutes.Value;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
