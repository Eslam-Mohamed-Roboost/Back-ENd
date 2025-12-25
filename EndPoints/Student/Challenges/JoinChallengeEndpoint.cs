using API.Application.Features.Student.Challenges.Commands;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Challenges;

public class JoinChallengeEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Challenges/{challengeId}/Join",
                async (IMediator mediator, [AsParameters] JoinChallengeRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new JoinChallengeCommand(routeParams.ChallengeId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

public class JoinChallengeRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ChallengeId { get; set; }
}
