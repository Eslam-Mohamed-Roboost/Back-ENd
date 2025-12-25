using API.Application.Features.Student.Portfolio.DTOs;
using API.Application.Features.Teacher.Portfolio.DTOs;
using API.Domain.Entities;
using API.Domain.Entities.General;
using API.Domain.Entities.Portfolio;
using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Portfolio.Queries;

public record GetStudentPortfolioQuery(long StudentId, long SubjectId) : IRequest<RequestResult<TeacherPortfolioDto>>;

public class GetStudentPortfolioQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Subjects> subjectsRepository,
    IRepository<PortfolioFiles> portfolioFilesRepository,
    IRepository<TeacherFeedback> feedbackRepository,
    IRepository<PortfolioLikes> likesRepository,
    IRepository<StudentBadges> studentBadgesRepository,
    IRepository<Badges> badgesRepository)
    : RequestHandlerBase<GetStudentPortfolioQuery, RequestResult<TeacherPortfolioDto>>(parameters)
{
    public override async Task<RequestResult<TeacherPortfolioDto>> Handle(GetStudentPortfolioQuery request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var student = await userRepository.Get(x => x.ID == request.StudentId).FirstOrDefaultAsync(cancellationToken);
        if (student == null)
            return RequestResult<TeacherPortfolioDto>.Failure(ErrorCode.NotFound, "Student not found");

        var subject = await subjectsRepository.Get(x => x.ID == request.SubjectId).FirstOrDefaultAsync(cancellationToken);
        if (subject == null)
            return RequestResult<TeacherPortfolioDto>.Failure(ErrorCode.NotFound, "Subject not found");

        var files = await portfolioFilesRepository.Get(x => x.StudentId == request.StudentId && x.SubjectId == request.SubjectId)
            .OrderByDescending(x => x.UploadedAt)
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

        var feedback = await (from f in feedbackRepository.Get()
                              join t in userRepository.Get() on f.TeacherId equals t.ID
                              where f.StudentId == request.StudentId && f.SubjectId == request.SubjectId
                              orderby f.CreatedAt descending
                              select new TeacherPortfolioCommentDto
                              {
                                  Id = f.ID,
                                  TeacherId = f.TeacherId,
                                  TeacherName = t.Name,
                                  Content = f.Comment,
                                  CreatedAt = f.CreatedAt,
                                  Type = f.Type.ToString().ToLowerInvariant()
                              })
            .ToListAsync(cancellationToken);

        var likesCount = await likesRepository.Get(x => x.StudentId == request.StudentId && x.SubjectId == request.SubjectId)
            .CountAsync(cancellationToken);

        var isLiked = await likesRepository.Get(x => x.StudentId == request.StudentId && x.SubjectId == request.SubjectId && x.TeacherId == teacherId)
            .AnyAsync(cancellationToken);

        // Portfolio-specific badges are not explicitly modeled; for now, return empty list.
        var badges = new List<PortfolioBadgeDto>();

        var lastUpdated = files.FirstOrDefault()?.UploadDate ?? feedback.FirstOrDefault()?.CreatedAt;

        var dto = new TeacherPortfolioDto
        {
            Id = request.StudentId,
            StudentId = request.StudentId,
            StudentName = student.Name,
            SubjectId = request.SubjectId,
            SubjectName = subject.Name,
            Submissions = files,
            Feedback = feedback,
            Badges = badges,
            Likes = likesCount,
            IsLiked = isLiked,
            LastUpdated = lastUpdated
        };

        return RequestResult<TeacherPortfolioDto>.Success(dto);
    }
}


