using API.Application.Features.Student.Missions.Commands;
using API.Application.Features.Student.Missions.DTOs;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Missions;

public class UpdateMissionProgressEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Missions/{missionId}/Progress",
                async (IMediator mediator, [AsParameters] MissionProgressRouteParams routeParams, UpdateMissionProgressRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateMissionProgressCommand(routeParams.MissionId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<MissionProgressResponse>>();
    }
}

public class MissionProgressRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
}
