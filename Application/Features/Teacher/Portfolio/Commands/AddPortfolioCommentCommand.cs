using API.Application.Features.Teacher.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Teacher.Portfolio.Commands;

public record AddPortfolioCommentCommand(long StudentId, long SubjectId, TeacherPortfolioCommentRequest Request) : IRequest<RequestResult<bool>>;

public class AddPortfolioCommentCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherFeedback> feedbackRepository)
    : RequestHandlerBase<AddPortfolioCommentCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(AddPortfolioCommentCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var entity = new TeacherFeedback
        {
            StudentId = request.StudentId,
            TeacherId = teacherId,
            SubjectId = request.SubjectId,
            Comment = request.Request.Content,
            Type = request.Request.Type.Equals("revision-request", StringComparison.OrdinalIgnoreCase)
                ? Domain.Enums.FeedbackType.RevisionRequest
                : Domain.Enums.FeedbackType.Comment,
            CreatedAt = DateTime.UtcNow
        };

        feedbackRepository.Add(entity);
        await feedbackRepository.SaveChangesAsync();

        return RequestResult<bool>.Success(true);
    }
}


