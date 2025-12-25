using API.Application.Features.Student.Progress.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Progress;

public class GetStreakEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Progress/Streak",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var query = new GetStreakQuery();
                    var result = await mediator.Send(query, cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<int>>();
    }
}

