using API.Application.Features.Admin.Challenges.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.ChallengesSeed;

public class ChallengesSeedEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/Challenges/Seed",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SeedChallengesCommand(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
          //s  .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<int>>();
    }
}


