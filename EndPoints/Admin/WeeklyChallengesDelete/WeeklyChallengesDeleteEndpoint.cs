using API.Application.Features.Admin.WeeklyChallenges.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.WeeklyChallengesDelete;

public class WeeklyChallengesDeleteEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Admin/WeeklyChallenges/{id}",
                async (IMediator mediator, long id, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new DeleteWeeklyChallengeCommand(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

