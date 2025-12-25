using API.Application.Features.Student.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Application.Features.Student.Portfolio.Queries;

public record GetSubjectPortfolioQuery(long SubjectId) : IRequest<RequestResult<SubjectPortfolioDto>>;

public class GetSubjectPortfolioQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioFiles> portfolioFilesRepository,
    IRepository<PortfolioReflections> reflectionsRepository,
    IRepository<TeacherFeedback> feedbackRepository,
    IRepository<Domain.Entities.Gamification.StudentBadges> studentBadgesRepository,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.General.Subjects> subjectsRepository)
    : RequestHandlerBase<GetSubjectPortfolioQuery, RequestResult<SubjectPortfolioDto>>(parameters)
{
    public override async Task<RequestResult<SubjectPortfolioDto>> Handle(GetSubjectPortfolioQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var subject = await subjectsRepository.Get(x => x.ID == request.SubjectId).FirstOrDefaultAsync(cancellationToken);
        if (subject == null)
            return RequestResult<SubjectPortfolioDto>.Failure(errorCode:ErrorCode.UserNotFound,"Subject not found");

        // Get files
        var files = await portfolioFilesRepository.Get(x => x.StudentId == studentId && x.SubjectId == request.SubjectId)
            .Select(x => new PortfolioFileDto
            {
                Id = x.ID,
                FileName = x.FileName,
                FileType = x.FileType.ToString(),
                FileSize = x.FileSize,
                UploadDate = x.UploadedAt,
                SubjectId = x.SubjectId,
                ThumbnailUrl = x.ThumbnailUrl,
                PreviewUrl = x.PreviewUrl,
                DownloadUrl = x.DownloadUrl
            })
            .ToListAsync(cancellationToken);

        // Get reflections
        var reflections = await reflectionsRepository.Get(x => x.StudentId == studentId && x.SubjectId == request.SubjectId)
            .Select(x => new ReflectionDto
            {
                Id = x.ID,
                Content = x.Content,
                Date = x.CreatedAt,
                Prompt = x.Prompt,
                SubjectId = x.SubjectId,
                AutoSaved = x.IsAutoSaved
            })
            .ToListAsync(cancellationToken);

        // Get feedback
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
            .ToListAsync(cancellationToken);

        var stats = new PortfolioStatsDto
        {
            FilesCount = files.Count,
            LatestUploadDate = files.Any() ? files.Max(x => x.UploadDate) : null,
            FeedbackCount = feedback.Count,
            BadgesCount = 0
        };

        var result = new SubjectPortfolioDto
        {
            SubjectId = subject.ID,
            SubjectName = subject.Name,
            SubjectIcon = subject.Icon ?? "",
            Files = files,
            Feedback = feedback,
            Reflections = reflections,
            Badges = new(),
            Stats = stats
        };

        return RequestResult<SubjectPortfolioDto>.Success(result);
    }
}
