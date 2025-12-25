using API.Application.Features.Admin.Teachers.Commands;
using API.Application.Features.Admin.Teachers.DTOs;
using API.Application.Features.Admin.Teachers.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Admin.Teachers;

public class TeacherAssignmentEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // POST /Admin/Teachers/{teacherId}/Assign - Assign teacher to classes and subjects
        app.MapPost("/Admin/Teachers/{teacherId}/Assign",
                async (long teacherId, [FromBody] TeacherAssignmentDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AssignTeacherToClassCommand(teacherId, request.Assignments);
                    var result = await mediator.Send(command, cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeacherAssignmentResponse>>();

        // GET /Admin/Teachers/{teacherId}/Assignments - Get teacher's class assignments
        app.MapGet("/Admin/Teachers/{teacherId}/Assignments",
                async (long teacherId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherAssignmentsQuery(teacherId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<TeacherAssignmentInfo>>>();
    }
}

