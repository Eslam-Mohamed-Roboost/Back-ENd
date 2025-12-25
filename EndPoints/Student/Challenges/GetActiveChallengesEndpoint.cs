using API.Application.Features.Student.Challenges.DTOs;
using API.Application.Features.Student.Challenges.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Challenges;

public class GetActiveChallengesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Challenges/Active",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetActiveChallengesQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ChallengeDto>>>();
    }
}
