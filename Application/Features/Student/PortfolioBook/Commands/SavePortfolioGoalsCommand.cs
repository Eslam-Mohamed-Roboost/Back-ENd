using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioGoalsCommand(SavePortfolioGoalsRequest Request) : IRequest<RequestResult<PortfolioGoalsDto>>;

public class SavePortfolioGoalsCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioBookGoals> goalsRepository)
    : RequestHandlerBase<SavePortfolioGoalsCommand, RequestResult<PortfolioGoalsDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioGoalsDto>> Handle(SavePortfolioGoalsCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var alreadySubmitted = await goalsRepository.Get(x => x.StudentId == studentId && x.SubjectId == request.Request.SubjectId)
            .AnyAsync(cancellationToken);

        if (alreadySubmitted)
        {
            return RequestResult<PortfolioGoalsDto>.Failure(ErrorCode.BadRequest, "You have already submitted your goals.");
        }

        var entity = new PortfolioBookGoals
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            AcademicGoal = request.Request.AcademicGoal,
            BehavioralGoal = request.Request.BehavioralGoal,
            PersonalGrowthGoal = request.Request.PersonalGrowthGoal,
            AchievementSteps = request.Request.AchievementSteps,
            TargetDate = request.Request.TargetDate
        };

        goalsRepository.Add(entity);
        await goalsRepository.SaveChangesAsync(cancellationToken);

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
