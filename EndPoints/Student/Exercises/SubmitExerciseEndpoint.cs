using API.Application.Features.Student.Exercises.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Exercises;

public class SubmitExerciseEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Exercises/{exerciseId}/Submit",
                async (IMediator mediator, long exerciseId, SubmitExerciseRequest request, CancellationToken cancellationToken) =>
                {
                    var command = new SubmitExerciseCommand(exerciseId, request);
                    var result = await mediator.Send(command, cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();
    }
}

