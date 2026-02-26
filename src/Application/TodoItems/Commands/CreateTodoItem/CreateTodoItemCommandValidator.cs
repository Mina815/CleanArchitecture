namespace CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;

public class CreateShopCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateShopCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}
