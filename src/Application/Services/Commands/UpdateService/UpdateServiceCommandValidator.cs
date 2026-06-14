namespace CleanArchitecture.Application.Services.Commands.UpdateService;

public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
    }
}
