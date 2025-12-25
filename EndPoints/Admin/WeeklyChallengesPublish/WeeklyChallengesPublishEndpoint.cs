using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Admin.WeeklyChallengesPublish;

public class WeeklyChallengesPublishEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/WeeklyChallenges/{id}/Publish",
                (long id, CancellationToken cancellationToken) =>
                {
                    var result1 =   RequestResult<bool>.Success(true,"Weekly challenge published successfully");
                    return Response(result1);
                 })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
