using API.Application.Features.Student.Goals.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Goals.Queries;

public record GetStudentGoalsQuery : IRequest<RequestResult<List<StudentGoalDto>>>;

public class GetStudentGoalsQueryHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<GetStudentGoalsQuery, RequestResult<List<StudentGoalDto>>>(parameters)
{
    public override async Task<RequestResult<List<StudentGoalDto>>> Handle(GetStudentGoalsQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement when StudentGoal entity is created
        // Placeholder return
        var goals = new List<StudentGoalDto>();

        return RequestResult<List<StudentGoalDto>>.Success(goals);
    }
}
