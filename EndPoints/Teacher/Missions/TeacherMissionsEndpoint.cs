using API.Application.Features.Teacher.Missions.DTOs;
using API.Application.Features.Teacher.Missions.Queries;
using API.Application.Features.Teacher.Missions.Commands;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Teacher.Missions;

public class TeacherMissionsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Teacher/Missions - List all available missions
        app.MapGet("/Teacher/Missions",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherMissionsQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<TeacherMissionDto>>>();

        // GET /Teacher/Missions/{missionId} - Get mission details
        app.MapGet("/Teacher/Missions/{missionId}",
                async (IMediator mediator, [AsParameters] TeacherMissionRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherMissionDetailsQuery(routeParams.MissionId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeacherMissionDetailDto>>();

        // GET /Teacher/Missions/Progress - Get teacher's progress summary
        app.MapGet("/Teacher/Missions/Progress",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherMissionsProgressQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeacherMissionsProgressSummaryDto>>();

        // POST /Teacher/Missions/{missionId}/Start - Start a mission
        app.MapPost("/Teacher/Missions/{missionId}/Start",
                async (IMediator mediator, [AsParameters] TeacherMissionRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new StartTeacherMissionCommand(routeParams.MissionId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeacherMissionProgressDto>>();

        // POST /Teacher/Missions/{missionId}/Progress - Update progress
        app.MapPost("/Teacher/Missions/{missionId}/Progress",
                async (IMediator mediator, [AsParameters] TeacherMissionRouteParams routeParams, UpdateTeacherMissionProgressRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateTeacherMissionProgressCommand(routeParams.MissionId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeacherMissionProgressResponse>>();
    }
}

public class TeacherMissionRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
}

