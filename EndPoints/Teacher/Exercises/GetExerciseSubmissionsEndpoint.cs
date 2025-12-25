using API.Application.Features.Teacher.Exercises.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Exercises;

public class GetExerciseSubmissionsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/Exercises/{exerciseId}/Submissions",
                async (IMediator mediator, long exerciseId, CancellationToken cancellationToken) =>
                {
                    var query = new GetExerciseSubmissionsQuery(exerciseId);
                    var result = await mediator.Send(query, cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<SubmissionDto>>>();
    }
}

