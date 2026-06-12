using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Branches.Commands.UpdateBranch;

[Authorize(Roles = Roles.Provider)]
public record UpdateBranchCommand : IRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string City { get; init; } = null!;
    public string? District { get; init; }
    public string Phone { get; init; } = null!;
    public string? WhatsappNumber { get; init; }
    public bool IsActive { get; init; } = true;
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
        var entity = await _context.Branches.Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (entity.Center.OwnerId != _user.Id) throw new ForbiddenAccessException();

        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.Address = request.Address;
        entity.City = request.City;
        entity.District = request.District;
        entity.Phone = request.Phone;
        entity.WhatsappNumber = request.WhatsappNumber;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
