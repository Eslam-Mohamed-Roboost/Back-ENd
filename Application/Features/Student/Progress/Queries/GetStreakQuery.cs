using API.Application.Services;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Progress.Queries;

public record GetStreakQuery : IRequest<RequestResult<int>>;

public class GetStreakQueryHandler(
    RequestHandlerBaseParameters parameters,
    IStreakCalculationService streakService)
    : RequestHandlerBase<GetStreakQuery, RequestResult<int>>(parameters)
{
    public override async Task<RequestResult<int>> Handle(
        GetStreakQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var streak = await streakService.CalculateStudentStreakAsync(studentId, cancellationToken);
        return RequestResult<int>.Success(streak);
    }
}

