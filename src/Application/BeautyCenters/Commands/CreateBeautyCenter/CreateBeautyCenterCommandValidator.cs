using FluentValidation;

namespace CleanArchitecture.Application.BeautyCenters.Commands.CreateBeautyCenter;

public class CreateBeautyCenterCommandValidator : AbstractValidator<CreateBeautyCenterCommand>
{
    public CreateBeautyCenterCommandValidator()
    {
        RuleFor(v => v.Name).MaximumLength(200).NotEmpty();
        RuleFor(v => v.NameAr).MaximumLength(200).NotEmpty();
        RuleFor(v => v.Description).MaximumLength(2000);
        RuleFor(v => v.DescriptionAr).MaximumLength(2000);
        RuleFor(v => v.LogoUrl).MaximumLength(500);
    }
}
