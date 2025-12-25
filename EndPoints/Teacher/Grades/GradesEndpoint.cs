using API.Application.Features.Teacher.Grades.Commands;
using API.Application.Features.Teacher.Grades.DTOs;
using API.Application.Features.Teacher.Grades.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Grades;

public class GradesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/Grades",
                async (IMediator mediator, long? studentId, long? classId, long? subjectId, string? term, int? year, string? status, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetGradesQuery(studentId, classId, subjectId, term, year, status), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<GradeDto>>>();

        app.MapGet("/Teacher/Grades/Student/{studentId}",
                async (IMediator mediator, long studentId, long? classId, long? subjectId, string? term, int? year, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetStudentGradesQuery(studentId, classId, subjectId, term, year), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<GradeDto>>>();

        app.MapPost("/Teacher/Grades",
                async (IMediator mediator, CreateGradeRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateGradeCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<GradeDto>>();

        app.MapPut("/Teacher/Grades/{id}",
                async (IMediator mediator, long id, UpdateGradeRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateGradeCommand(id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<GradeDto>>();

        app.MapPost("/Teacher/Grades/{id}/Approve",
                async (IMediator mediator, long id, GradeApprovalRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new ApproveGradeCommand(id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<GradeDto>>();

        app.MapGet("/Teacher/Grades/Summary",
                async (IMediator mediator, long classId, long subjectId, string? term, int? year, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetGradeSummaryQuery(classId, subjectId, term, year), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<GradeSummaryDto>>();
    }
}

