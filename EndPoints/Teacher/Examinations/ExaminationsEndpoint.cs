using API.Application.Features.Teacher.Examinations.Commands;
using API.Application.Features.Teacher.Examinations.DTOs;
using API.Application.Features.Teacher.Examinations.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Examinations;

public class ExaminationsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/Examinations",
                async (IMediator mediator, long? classId, long? subjectId, string? type, string? status, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetExaminationsQuery(classId, subjectId, type, status), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ExaminationDto>>>();

        app.MapGet("/Teacher/Examinations/{id}",
                async (IMediator mediator, long id, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetExaminationByIdQuery(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ExaminationDto>>();

        app.MapPost("/Teacher/Examinations",
                async (IMediator mediator, CreateExaminationRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateExaminationCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ExaminationDto>>();

        app.MapPut("/Teacher/Examinations/{id}",
                async (IMediator mediator, long id, UpdateExaminationRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateExaminationCommand(id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ExaminationDto>>();

        app.MapDelete("/Teacher/Examinations/{id}",
                async (IMediator mediator, long id, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new DeleteExaminationCommand(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        app.MapGet("/Teacher/Examinations/{id}/Attempts",
                async (IMediator mediator, long id, string? status, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetExaminationAttemptsQuery(id, status), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ExaminationAttemptDto>>>();

        app.MapPost("/Teacher/Examinations/{id}/Grade",
                async (IMediator mediator, long id, long attemptId, GradeExaminationAttemptRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GradeExaminationAttemptCommand(attemptId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ExaminationAttemptDto>>();
    }
}

