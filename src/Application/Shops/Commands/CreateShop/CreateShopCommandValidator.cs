using CleanArchitecture.Application.Shops.Commands.CreateShop;

namespace CleanArchitecture.Application.Shops.Commands.CreateShop;

public class CreateShopCommandValidator : AbstractValidator<CreateShopCommand>
{
    public CreateShopCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(200)
            .NotEmpty();
    }
}
