using API.Application.Features.Student.Missions.DTOs;
using API.Application.Features.Student.Missions.Queries;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Missions;

public class GetMissionDetailsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Missions/{missionId}",
                async (IMediator mediator, [AsParameters] MissionDetailsRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetMissionDetailsQuery(request.MissionId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<MissionDetailDto>>();
    }
}

public class MissionDetailsRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
}
