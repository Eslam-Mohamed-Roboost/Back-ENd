using API.Application.Features.Student.Exercises.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Exercises;

public class GetAssignedExercisesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Exercises",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var query = new GetAssignedExercisesQuery();
                    var result = await mediator.Send(query, cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ExerciseDto>>>();
    }
}

