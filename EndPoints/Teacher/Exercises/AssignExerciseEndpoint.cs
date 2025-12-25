using API.Application.Features.Teacher.Exercises.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Exercises;

public class AssignExerciseEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Teacher/Exercises/Assign",
                async (IMediator mediator, AssignExerciseRequest request, CancellationToken cancellationToken) =>
                {
                    var command = new AssignExerciseCommand(request);
                    var result = await mediator.Send(command, cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();
    }
}

