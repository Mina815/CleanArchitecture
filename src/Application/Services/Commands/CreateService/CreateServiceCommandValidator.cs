namespace CleanArchitecture.Application.Services.Commands.CreateService;

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(v => v.CenterId).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Price).GreaterThanOrEqualTo(0);
        RuleFor(v => v.DurationMinutes).GreaterThan(0);
    }
}
