namespace CleanArchitecture.Application.Centers.Commands.CreateCenter;

public class CreateCenterCommandValidator : AbstractValidator<CreateCenterCommand>
{
    public CreateCenterCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.NameAr)
            .NotEmpty()
            .MaximumLength(200);
    }
}
