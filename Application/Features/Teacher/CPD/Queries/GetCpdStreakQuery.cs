using API.Application.Services;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Teacher.CPD.Queries;

public record GetCpdStreakQuery : IRequest<RequestResult<int>>;

public class GetCpdStreakQueryHandler(
    RequestHandlerBaseParameters parameters,
    IStreakCalculationService streakService)
    : RequestHandlerBase<GetCpdStreakQuery, RequestResult<int>>(parameters)
{
    public override async Task<RequestResult<int>> Handle(
        GetCpdStreakQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;
        var streak = await streakService.CalculateTeacherStreakAsync(teacherId, cancellationToken);
        return RequestResult<int>.Success(streak);
    }
}

