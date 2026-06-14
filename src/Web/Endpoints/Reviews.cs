using CleanArchitecture.Application.Reviews.Queries.GetCenterReviews;

namespace CleanArchitecture.Web.Endpoints;

public class Reviews : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetReviews, "center/{centerId}");
    }

    public static async Task<Ok<PaginatedList<ReviewDto>>> GetReviews(ISender sender, int centerId, int pageNumber = 1, int pageSize = 10)
    {
        var result = await sender.Send(new GetCenterReviewsQuery { CenterId = centerId, PageNumber = pageNumber, PageSize = pageSize });
        return TypedResults.Ok(result);
    }
}
