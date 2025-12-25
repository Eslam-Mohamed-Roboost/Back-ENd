using API.Application.Features.Student.Leaderboard.Queries;
using API.Application.Services;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Leaderboard;

public class GetMyPositionEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Leaderboard/MyPosition",
                async (IMediator mediator, LeaderboardType type, TimeRange range, CancellationToken cancellationToken) =>
                {
                    var query = new GetMyPositionQuery(type, range);
                    var result = await mediator.Send(query, cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<LeaderboardEntry?>>();
    }
}

