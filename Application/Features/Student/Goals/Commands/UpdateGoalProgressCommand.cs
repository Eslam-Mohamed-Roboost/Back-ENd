using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Goals.Commands;

public record UpdateGoalProgressCommand(long GoalId) : IRequest<RequestResult<bool>>;

public class UpdateGoalProgressCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<UpdateGoalProgressCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(UpdateGoalProgressCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement when StudentGoal entity is created
        // This should automatically track progress based on student activities
        return RequestResult<bool>.Success(true);
    }
}
