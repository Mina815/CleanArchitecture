using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.Shops.Commands.CreateShop;

public record CreateShopCommand : IRequest<int>
{
    public string? Name { get; set; }            
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}

public class CreateShopCommandHandler : IRequestHandler<CreateShopCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateShopCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        var entity = new Shop
        {
            Name = request.Name,
            Description = request.Description,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            isOpen = false
        };

        entity.AddDomainEvent(new ShopCreatedEvent(entity));

        _context.Shops.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
