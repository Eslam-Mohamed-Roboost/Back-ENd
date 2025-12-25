using API.Application.Features.Teacher.Exercises.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Exercises;

public class GradeExerciseEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Teacher/Exercises/{submissionId}/Grade",
                async (IMediator mediator, long submissionId, GradeExerciseRequest request, CancellationToken cancellationToken) =>
                {
                    var command = new GradeExerciseCommand(submissionId, request);
                    var result = await mediator.Send(command, cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

