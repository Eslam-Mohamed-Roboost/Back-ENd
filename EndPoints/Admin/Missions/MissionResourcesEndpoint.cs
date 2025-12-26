using API.Application.Features.Admin.Missions.Commands;
using API.Application.Features.Admin.Missions.DTOs;
using API.Application.Features.Admin.Missions.Queries;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace API.EndPoints.Admin.Missions;

public class MissionResourcesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // POST /Admin/Missions/{missionId}/Resources/Upload - Upload file (MUST be registered BEFORE the general /Resources route)
        app.MapPost("/Admin/Missions/{missionId}/Resources/Upload",
                async (IMediator mediator, string missionId, [FromForm] UploadMissionResourceRequest request, CancellationToken cancellationToken) =>
                {
                    // Parse missionId from route
                    if (!long.TryParse(missionId, out var missionIdLong))
                    {
                        return Results.BadRequest(new { Message = "Invalid mission ID format" });
                    }
                    
                    request.MissionId = missionIdLong; // Set missionId from route
                    var result = await mediator.Send(new UploadMissionResourceCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .DisableAntiforgery()
            .Produces<EndPointResponse<long>>();

        // GET /Admin/Missions/{missionId}/Resources
        app.MapGet("/Admin/Missions/{missionId}/Resources",
                async (IMediator mediator, [AsParameters] MissionResourcesRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetMissionResourcesQuery(routeParams.MissionId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<MissionResourceDto>>>();

        // POST /Admin/Missions/{missionId}/Resources - Create with URL
        app.MapPost("/Admin/Missions/{missionId}/Resources",
                async (IMediator mediator, [AsParameters] MissionResourcesRouteParams routeParams, [FromBody] CreateMissionResourceDto request, CancellationToken cancellationToken) =>
                {
                    request.MissionId = routeParams.MissionId; // Ensure missionId matches route
                    var result = await mediator.Send(new CreateMissionResourceCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();

        // PUT /Admin/Missions/Resources/{resourceId}
        app.MapPut("/Admin/Missions/Resources/{resourceId}",
                async (IMediator mediator, [AsParameters] MissionResourceIdRouteParams routeParams, [FromBody] UpdateMissionResourceDto request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateMissionResourceCommand(routeParams.ResourceId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // DELETE /Admin/Missions/Resources/{resourceId}
        app.MapDelete("/Admin/Missions/Resources/{resourceId}",
                async (IMediator mediator, [AsParameters] MissionResourceIdRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new DeleteMissionResourceCommand(routeParams.ResourceId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

public class MissionResourcesRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
}

public class MissionResourceIdRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ResourceId { get; set; }
}

