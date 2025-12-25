using API.Application.Features.Student.Leaderboard.Queries;
using API.Application.Services;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Leaderboard;

public class GetLeaderboardEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Leaderboard",
                async (IMediator mediator, LeaderboardType type, TimeRange range, int limit, CancellationToken cancellationToken) =>
                {
                    var query = new GetLeaderboardQuery(type, range, limit);
                    var result = await mediator.Send(query, cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<LeaderboardEntry>>>();
    }
}

