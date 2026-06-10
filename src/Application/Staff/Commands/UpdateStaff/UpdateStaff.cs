using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Staff.Commands.UpdateStaff;

[Authorize(Roles = Roles.Provider)]
public record UpdateStaffCommand : IRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Phone { get; init; }
    public string? Specialization { get; init; }
    public bool IsActive { get; init; } = true;
}

public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public UpdateStaffCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.StaffMembers.Include(s => s.Branch).ThenInclude(b => b.Center)
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.Branch.Center.OwnerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name;
        entity.Phone = request.Phone;
        entity.Specialization = request.Specialization;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
