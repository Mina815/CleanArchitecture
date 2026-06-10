using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Services.Commands.DeleteService;

[Authorize(Roles = Roles.Provider)]
public record DeleteServiceCommand(int Id) : IRequest;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public DeleteServiceCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CenterServices.Include(s => s.Center)
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.Center.OwnerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
