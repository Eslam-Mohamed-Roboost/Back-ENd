using API.Application.Features.Admin.WeeklyChallenges.Commands;
using API.Application.Features.Admin.WeeklyChallenges.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.WeeklyChallengesCreate;

public class WeeklyChallengesCreateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/WeeklyChallenges",
                async (IMediator mediator, CreateWeeklyChallengeDto request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateWeeklyChallengeCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();
    }
}
