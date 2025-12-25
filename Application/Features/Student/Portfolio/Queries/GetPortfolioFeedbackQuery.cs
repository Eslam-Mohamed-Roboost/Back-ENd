using API.Application.Features.Student.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Portfolio.Queries;

public record GetPortfolioFeedbackQuery(long SubjectId) : IRequest<RequestResult<List<TeacherFeedbackDto>>>;

public class GetPortfolioFeedbackQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherFeedback> feedbackRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetPortfolioFeedbackQuery, RequestResult<List<TeacherFeedbackDto>>>(parameters)
{
    public override async Task<RequestResult<List<TeacherFeedbackDto>>> Handle(GetPortfolioFeedbackQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var feedback = await (from f in feedbackRepository.Get()
                             join u in userRepository.Get() on f.TeacherId equals u.ID
                             where f.StudentId == studentId && f.SubjectId == request.SubjectId
                             select new TeacherFeedbackDto
                             {
                                 Id = f.ID,
                                 TeacherName = u.Name,
                                 Date = f.CreatedAt,
                                 Comment = f.Comment,
                                 RelatedFileId = f.FileId
                             })
            .OrderByDescending(x => x.Date)
            .ToListAsync(cancellationToken);

        return RequestResult<List<TeacherFeedbackDto>>.Success(feedback);
    }
}
