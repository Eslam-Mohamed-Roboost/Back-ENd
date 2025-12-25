using API.Application.Features.Student.Goals.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Goals.Commands;

public record CreateGoalCommand(CreateGoalRequest Request) : IRequest<RequestResult<StudentGoalDto>>;

public class CreateGoalCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<CreateGoalCommand, RequestResult<StudentGoalDto>>(parameters)
{
    public override async Task<RequestResult<StudentGoalDto>> Handle(CreateGoalCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement when StudentGoal entity is created
        // Placeholder return
        var goal = new StudentGoalDto
        {
            Id = 1,
            Title = request.Request.Title,
            Description = request.Request.Description,
            Type = request.Request.Type,
            StartDate = DateTime.UtcNow,
            EndDate = request.Request.EndDate,
            Status = "active",
            CurrentProgress = 0,
            TargetProgress = request.Request.TargetProgress,
            PercentageComplete = 0,
            Category = request.Request.Category
        };

        return RequestResult<StudentGoalDto>.Success(goal);
    }
}
