using API.Application.Features.Student.Goals.Commands;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Goals;

public class UpdateGoalProgressEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Student/Goals/{goalId}/Progress",
                async (IMediator mediator, [AsParameters] UpdateGoalRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateGoalProgressCommand(routeParams.GoalId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

public class UpdateGoalRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long GoalId { get; set; }
}
