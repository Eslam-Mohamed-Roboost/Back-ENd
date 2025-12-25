using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioGoalsCommand(SavePortfolioGoalsRequest Request) : IRequest<RequestResult<PortfolioGoalsDto>>;

public class SavePortfolioGoalsCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<SavePortfolioGoalsCommand, RequestResult<PortfolioGoalsDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioGoalsDto>> Handle(SavePortfolioGoalsCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement database save when entity is created
        var goals = new PortfolioGoalsDto
        {
            AcademicGoal = request.Request.AcademicGoal,
            BehavioralGoal = request.Request.BehavioralGoal,
            PersonalGrowthGoal = request.Request.PersonalGrowthGoal,
            AchievementSteps = request.Request.AchievementSteps,
            TargetDate = request.Request.TargetDate
        };

        return RequestResult<PortfolioGoalsDto>.Success(goals);
    }
}
