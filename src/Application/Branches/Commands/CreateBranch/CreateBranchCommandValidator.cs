namespace CleanArchitecture.Application.Branches.Commands.CreateBranch;

public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(v => v.CenterId).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Address).NotEmpty().MaximumLength(500);
        RuleFor(v => v.City).NotEmpty().MaximumLength(100);
    }
}
