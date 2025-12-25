using API.Application.Features.Student.Challenges.Commands;
using API.Application.Features.Student.Challenges.DTOs;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Challenges;

public class SubmitChallengeEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Challenges/{challengeId}/Submit",
                async (IMediator mediator, [AsParameters] SubmitChallengeRouteParams routeParams, SubmitChallengeRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SubmitChallengeCommand(routeParams.ChallengeId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ChallengeSubmissionResponse>>();
    }
}

public class SubmitChallengeRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ChallengeId { get; set; }
}
