using API.Application.Services;
using API.Domain.Enums;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Leaderboard.Queries;

public record GetMyPositionQuery(
    LeaderboardType Type,
    TimeRange Range) : IRequest<RequestResult<LeaderboardEntry?>>;

public class GetMyPositionQueryHandler(
    RequestHandlerBaseParameters parameters,
    ILeaderboardService leaderboardService)
    : RequestHandlerBase<GetMyPositionQuery, RequestResult<LeaderboardEntry?>>(parameters)
{
    public override async Task<RequestResult<LeaderboardEntry?>> Handle(
        GetMyPositionQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var position = await leaderboardService.GetUserPositionAsync(
            studentId,
            request.Type,
            request.Range,
            cancellationToken);

        if (position == null)
        {
            return RequestResult<LeaderboardEntry?>.Failure(
                ErrorCode.NotFound,
                "You are not ranked yet. Complete more activities to appear on the leaderboard!");
        }

        return RequestResult<LeaderboardEntry?>.Success(position);
    }
}

