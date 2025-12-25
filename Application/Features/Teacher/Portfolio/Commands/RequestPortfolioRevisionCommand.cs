using API.Application.Features.Teacher.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Teacher.Portfolio.Commands;

public record RequestPortfolioRevisionCommand(long StudentId, long SubjectId, TeacherPortfolioRevisionRequest Request) : IRequest<RequestResult<bool>>;

public class RequestPortfolioRevisionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherFeedback> feedbackRepository,
    IRepository<PortfolioStatus> statusRepository)
    : RequestHandlerBase<RequestPortfolioRevisionCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(RequestPortfolioRevisionCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var feedback = new TeacherFeedback
        {
            StudentId = request.StudentId,
            TeacherId = teacherId,
            SubjectId = request.SubjectId,
            Comment = request.Request.Feedback,
            Type = FeedbackType.RevisionRequest,
            CreatedAt = DateTime.UtcNow
        };

        feedbackRepository.Add(feedback);

        var status = new PortfolioStatus
        {
            StudentId = request.StudentId,
            SubjectId = request.SubjectId,
            Status = ReviewStatus.NeedsRevision,
            LastReviewedBy = teacherId,
            LastReviewedAt = DateTime.UtcNow
        };

        statusRepository.Add(status);

        await feedbackRepository.SaveChangesAsync();
        await statusRepository.SaveChangesAsync();

        return RequestResult<bool>.Success(true);
    }
}


