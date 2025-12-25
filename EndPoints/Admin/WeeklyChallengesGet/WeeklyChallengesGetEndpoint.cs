using API.Application.Features.Admin.WeeklyChallenges.DTOs;
using API.Application.Features.Admin.WeeklyChallenges.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.WeeklyChallengesGet;

public class WeeklyChallengesGetEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/WeeklyChallenges",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetWeeklyChallengesQuery(), cancellationToken);
                    return Response(result);
                 })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<WeeklyChallengeDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
