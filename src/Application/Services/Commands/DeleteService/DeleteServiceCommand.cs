using CleanArchitecture.Application.Common.Security;

namespace CleanArchitecture.Application.Services.Commands.DeleteService;

[Authorize(Roles = "Provider")]
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
        var entity = await _context.Services
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        var center = await _context.BeautyCenters
            .FindAsync([entity!.CenterId], cancellationToken);

        if (center!.OwnerId != _user.Id)
            throw new ForbiddenAccessException();

        _context.Services.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
