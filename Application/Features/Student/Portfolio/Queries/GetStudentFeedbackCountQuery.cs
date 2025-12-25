using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Portfolio.Queries;

public record GetStudentFeedbackCountQuery : IRequest<RequestResult<int>>;

public class GetStudentFeedbackCountQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherFeedback> feedbackRepository)
    : RequestHandlerBase<GetStudentFeedbackCountQuery, RequestResult<int>>(parameters)
{
    public override async Task<RequestResult<int>> Handle(GetStudentFeedbackCountQuery request, CancellationToken cancellationToken)
    {
        var count = await feedbackRepository.Get(x => x.StudentId == _userState.UserID)
            .CountAsync(cancellationToken);
        
        return RequestResult<int>.Success(count);
    }
}
