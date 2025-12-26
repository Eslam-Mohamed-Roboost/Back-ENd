using API.Application.Features.Admin.Missions.Commands;
using API.Application.Features.Admin.Missions.DTOs;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace API.EndPoints.Admin.MissionsUpdate;

public class MissionsUpdateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/Missions/{id}",
                async (IMediator mediator, [AsParameters] MissionUpdateRouteParams routeParams, [FromBody] UpdateMissionDto request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateMissionCommand(routeParams.Id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

public class MissionUpdateRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
}

