using API.Application.Features.Teacher.Exercises.Commands;
using API.Application.Features.Teacher.Exercises.DTOs;
using API.Application.Features.Teacher.Exercises.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Exercises;

public class ExercisesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/Exercises",
                async (IMediator mediator, long? classId, long? subjectId, string? type, string? status, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetExercisesQuery(classId, subjectId, type, status), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ExerciseDto>>>();

        app.MapGet("/Teacher/Exercises/{id}",
                async (IMediator mediator, long id, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetExerciseByIdQuery(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ExerciseDto>>();

        app.MapPost("/Teacher/Exercises",
                async (IMediator mediator, CreateExerciseRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateExerciseCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ExerciseDto>>();

        app.MapPut("/Teacher/Exercises/{id}",
                async (IMediator mediator, long id, UpdateExerciseRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateExerciseCommand(id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ExerciseDto>>();

        app.MapDelete("/Teacher/Exercises/{id}",
                async (IMediator mediator, long id, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new DeleteExerciseCommand(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        app.MapGet("/Teacher/Exercises/{id}/Submissions",
                async (IMediator mediator, long id, string? status, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetExerciseSubmissionsQuery(id, status), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ExerciseSubmissionDto>>>();

        app.MapPost("/Teacher/Exercises/{id}/Grade",
                async (IMediator mediator, long id, long submissionId, GradeExerciseSubmissionRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GradeExerciseSubmissionCommand(submissionId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ExerciseSubmissionDto>>();
    }
}

