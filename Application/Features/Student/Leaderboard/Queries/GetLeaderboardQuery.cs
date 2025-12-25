using API.Application.Services;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Leaderboard.Queries;

public record GetLeaderboardQuery(
    LeaderboardType Type,
    TimeRange Range,
    int Limit = 100) : IRequest<RequestResult<List<LeaderboardEntry>>>;

public class GetLeaderboardQueryHandler(
    RequestHandlerBaseParameters parameters,
    ILeaderboardService leaderboardService)
    : RequestHandlerBase<GetLeaderboardQuery, RequestResult<List<LeaderboardEntry>>>(parameters)
{
    public override async Task<RequestResult<List<LeaderboardEntry>>> Handle(
        GetLeaderboardQuery request,
        CancellationToken cancellationToken)
    {
        var leaderboard = await leaderboardService.GetStudentLeaderboardAsync(
            request.Type,
            request.Range,
            request.Limit,
            cancellationToken);

        return RequestResult<List<LeaderboardEntry>>.Success(leaderboard);
    }
}

