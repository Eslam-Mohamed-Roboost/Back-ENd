using API.Application.Features.Admin.WeeklyChallenges.DTOs;
using API.Application.Features.Admin.WeeklyChallenges.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.WeeklyChallengesGetById;

public class WeeklyChallengesGetByIdEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/WeeklyChallenges/{id}",
                async (long id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetWeeklyChallengeByIdQuery(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<WeeklyChallengeDto>>();
    }
}

