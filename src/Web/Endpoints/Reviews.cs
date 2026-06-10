using CleanArchitecture.Application.Reviews.Commands.CreateReview;
using CleanArchitecture.Application.Reviews.Queries.GetCenterReviews;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Reviews : IEndpointGroup
{
    public static string? RoutePrefix => "/api/reviews";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCenterReviews, "center/{centerId}");
        groupBuilder.MapPost(CreateReview).RequireAuthorization();
    }

    public static async Task<Ok<ReviewsVm>> GetCenterReviews(ISender sender, int centerId, int page = 1, int pageSize = 10)
        => TypedResults.Ok(await sender.Send(new GetCenterReviewsQuery(centerId, page, pageSize)));

    public static async Task<Created<int>> CreateReview(ISender sender, CreateReviewCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/reviews/{id}", id);
    }
}
