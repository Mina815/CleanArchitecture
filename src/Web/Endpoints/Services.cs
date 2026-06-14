using CleanArchitecture.Application.Services.Queries.GetCenterServices;

namespace CleanArchitecture.Web.Endpoints;

public class Services : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetServices, "center/{centerId}");
    }

    public static async Task<Ok<List<ServiceDto>>> GetServices(ISender sender, int centerId)
    {
        var result = await sender.Send(new GetCenterServicesQuery(centerId));
        return TypedResults.Ok(result);
    }
}
