using CleanArchitecture.Application.Shops.Queries.GetShopsWithPagination;

namespace CleanArchitecture.Application.Shops.Queries.GetTodoItemsWithPagination;

public class GetShopsWithPaginationQueryValidator : AbstractValidator<GetShopsWithPaginationQuery>
{
    public GetShopsWithPaginationQueryValidator()
    {

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}
