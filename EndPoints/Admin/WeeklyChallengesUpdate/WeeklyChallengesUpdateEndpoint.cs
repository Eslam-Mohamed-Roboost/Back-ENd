using API.Application.Features.Admin.WeeklyChallenges.DTOs;
using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Admin.WeeklyChallengesUpdate;

public class WeeklyChallengesUpdateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/WeeklyChallenges/{id}",
                (long id, CreateWeeklyChallengeDto request, CancellationToken cancellationToken) =>
                {
                    var result1 =   RequestResult<bool>.Success(true,"Weekly challenge updated successfully");
                    return Response(result1);
                 })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
