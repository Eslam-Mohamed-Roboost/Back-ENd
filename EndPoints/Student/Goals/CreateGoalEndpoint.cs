using API.Application.Features.Student.Goals.Commands;
using API.Application.Features.Student.Goals.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Goals;

public class CreateGoalEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Goals",
                async (IMediator mediator, CreateGoalRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateGoalCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<StudentGoalDto>>();
    }
}
