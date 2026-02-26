using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace CleanArchitecture.Application.Shops.Queries.GetShopsWithPagination;

public record GetShopsWithPaginationQuery : IRequest<PaginatedList<ShopBriefDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetShopsWithPaginationQueryHandler : IRequestHandler<GetShopsWithPaginationQuery, PaginatedList<ShopBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetShopsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ShopBriefDto>> Handle(GetShopsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Shops
            .OrderBy(x => x.Id)
            .ProjectTo<ShopBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
