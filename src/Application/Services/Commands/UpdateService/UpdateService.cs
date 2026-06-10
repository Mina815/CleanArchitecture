using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Services.Commands.UpdateService;

[Authorize(Roles = Roles.Provider)]
public record UpdateServiceCommand : IRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
    public bool IsActive { get; init; } = true;
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
        var entity = await _context.CenterServices.Include(s => s.Center)
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.Center.OwnerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.Price = request.Price;
        entity.DurationMinutes = request.DurationMinutes;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
