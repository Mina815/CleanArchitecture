using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Branches.Commands.CreateBranch;

[Authorize(Roles = Roles.Provider)]
public record CreateBranchCommand : IRequest<int>
{
    public int CenterId { get; init; }
    public string Name { get; init; } = null!;
    public string NameAr { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string City { get; init; } = null!;
    public string? District { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string Phone { get; init; } = null!;
    public string? WhatsappNumber { get; init; }
}

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateBranchCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var center = await _context.BeautyCenters
            .FirstOrDefaultAsync(c => c.Id == request.CenterId, cancellationToken);
        Guard.Against.NotFound(request.CenterId, center);

        if (center.OwnerId != _user.Id) throw new ForbiddenAccessException();

        var entity = new Branch
        {
            CenterId = request.CenterId,
            Name = request.Name,
            NameAr = request.NameAr,
            Address = request.Address,
            City = request.City,
            District = request.District,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Phone = request.Phone,
            WhatsappNumber = request.WhatsappNumber,
            IsActive = true
        };

        _context.Branches.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
