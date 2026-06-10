using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using StaffEntity = CleanArchitecture.Domain.Entities.Staff;

namespace CleanArchitecture.Application.Staff.Commands.CreateStaff;

[Authorize(Roles = Roles.Provider)]
public record CreateStaffCommand : IRequest<int>
{
    public int BranchId { get; init; }
    public string Name { get; init; } = null!;
    public string? Phone { get; init; }
    public string? ImageUrl { get; init; }
    public string? Specialization { get; init; }
}

public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateStaffCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches.Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == request.BranchId && b.Center.OwnerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.BranchId, branch);

        var entity = new StaffEntity
        {
            BranchId = request.BranchId,
            Name = request.Name,
            Phone = request.Phone,
            ImageUrl = request.ImageUrl,
            Specialization = request.Specialization,
            IsActive = true
        };

        _context.StaffMembers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
