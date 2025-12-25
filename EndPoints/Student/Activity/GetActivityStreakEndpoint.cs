using API.Application.Features.Student.Activity.DTOs;
using API.Application.Features.Student.Activity.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Activity;

public class GetActivityStreakEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Activity/Streak",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetActivityStreakQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ActivityStreakDto>>();
    }
}
