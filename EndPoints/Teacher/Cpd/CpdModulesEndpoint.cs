using API.Application.Features.Teacher.Cpd.DTOs;
using API.Application.Features.Teacher.Cpd.Queries;
using API.Application.Features.Teacher.Cpd.Commands;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace API.EndPoints.Teacher.Cpd;

public class CpdModulesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Teacher/Cpd/Modules
        app.MapGet("/Teacher/Cpd/Modules",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCpdModulesQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<CpdModuleDto>>>();

        // GET /Teacher/Cpd/Modules/{id}
        app.MapGet("/Teacher/Cpd/Modules/{id}",
                async (IMediator mediator, [AsParameters] GetModuleRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCpdModuleByIdQuery(routeParams.Id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<CpdModuleDto>>();

        // GET /Teacher/Cpd/Progress
        app.MapGet("/Teacher/Cpd/Progress",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCpdProgressQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<CpdProgressDto>>();

        // POST /Teacher/Cpd/Modules/{id}/Status
        app.MapPost("/Teacher/Cpd/Modules/{id}/Status",
                async (IMediator mediator, [AsParameters] GetModuleRouteParams routeParams, [FromBody] UpdateCpdModuleStatusRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateCpdModuleStatusCommand(routeParams.Id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<CpdModuleDto>>();

        // POST /Teacher/Cpd/Modules/{id}/Evidence
        app.MapPost("/Teacher/Cpd/Modules/{id}/Evidence",
                async (IMediator mediator, [AsParameters] GetModuleRouteParams routeParams, [FromForm] UploadCpdEvidenceRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UploadCpdEvidenceCommand(routeParams.Id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .DisableAntiforgery()
            .Produces<EndPointResponse<CpdModuleDto>>();

        // GET /Teacher/CPD/Hours
        app.MapGet("/Teacher/CPD/Hours",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCpdHoursQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<CpdHoursSummaryDto>>();

        // GET /Teacher/CPD/Hours/Summary
        app.MapGet("/Teacher/CPD/Hours/Summary",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCpdHoursQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<CpdHoursSummaryDto>>();
    }
}

public class GetModuleRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
}


