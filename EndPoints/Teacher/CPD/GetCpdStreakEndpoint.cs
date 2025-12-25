using API.Application.Features.Teacher.CPD.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.CPD;

public class GetCpdStreakEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/CPD/Streak",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var query = new GetCpdStreakQuery();
                    var result = await mediator.Send(query, cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<int>>();
    }
}

