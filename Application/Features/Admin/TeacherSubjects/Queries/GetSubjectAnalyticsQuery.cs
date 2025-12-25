using API.Application.Features.Admin.TeacherSubjects.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.TeacherSubjects.Queries;

public record GetSubjectAnalyticsQuery(string? Subject) : IRequest<RequestResult<SubjectAnalyticsDto>>;

public class GetSubjectAnalyticsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Teacher.TeacherSubjects> teacherSubjectsRepository,
    IRepository<Domain.Entities.General.Subjects> subjectsRepository,
    IRepository<Domain.Entities.Portfolio.PortfolioFiles> portfolioRepository,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository,
    IRepository<Domain.Entities.Portfolio.TeacherFeedback> teacherFeedbackRepository)
    : RequestHandlerBase<GetSubjectAnalyticsQuery, RequestResult<SubjectAnalyticsDto>>(parameters)
{
    public override async Task<RequestResult<SubjectAnalyticsDto>> Handle(GetSubjectAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var subjectName = request.Subject ?? "All Subjects";
        long? subjectId = null;

        // Get subject ID if specific subject is requested
        if (!string.IsNullOrWhiteSpace(request.Subject))
        {
            var subject = await subjectsRepository.Get()
                .FirstOrDefaultAsync(s => s.Name.ToLower().Contains(request.Subject.ToLower()), cancellationToken);
            
            if (subject == null)
            {
                return RequestResult<SubjectAnalyticsDto>.Success(new SubjectAnalyticsDto
                {
                    Subject = request.Subject,
                    TeacherCount = 0,
                    StudentCount = 0,
                    PortfolioCompletionRate = 0,
                    CpdBadgeCompletionRate = 0,
                    ResourceUsage = new ResourceUsageStatsDto(),
                    TopTeachers = new List<TeacherPerformanceDto>()
                });
            }

            subjectId = subject.ID;
            subjectName = subject.Name;
        }

        // Get teacher count for the subject
        var teacherIdsQuery = teacherSubjectsRepository.Get().Select(ts => ts.TeacherId);
        if (subjectId.HasValue)
        {
            teacherIdsQuery = teacherSubjectsRepository.Get()
                .Where(ts => ts.SubjectId == subjectId.Value)
                .Select(ts => ts.TeacherId);
        }

        var teacherIds = await teacherIdsQuery.Distinct().ToListAsync(cancellationToken);
        var teacherCount = teacherIds.Count;

        // Get student count for the subject
        var studentCount = 0;
        if (subjectId.HasValue)
        {
            studentCount = await portfolioRepository.Get()
                .Where(p => p.SubjectId == subjectId.Value)
                .Select(p => p.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);
        }
        else
        {
            studentCount = await userRepository.Get()
                .CountAsync(u => u.Role == ApplicationRole.Student, cancellationToken);
        }

        // Calculate portfolio completion rate
        var totalStudents = await userRepository.Get()
            .CountAsync(u => u.Role == ApplicationRole.Student, cancellationToken);

        var studentsWithPortfolios = subjectId.HasValue
            ? await portfolioRepository.Get()
                .Where(p => p.SubjectId == subjectId.Value)
                .Select(p => p.StudentId)
                .Distinct()
                .CountAsync(cancellationToken)
            : await portfolioRepository.Get()
                .Select(p => p.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

        var portfolioCompletionRate = totalStudents > 0 ? (int)((studentsWithPortfolios * 100.0) / totalStudents) : 0;

        // Calculate CPD badge completion rate
        var totalTeachers = await userRepository.Get()
            .CountAsync(u => u.Role == ApplicationRole.Teacher, cancellationToken);

        var teachersWithCpdBadges = await cpdProgressRepository.Get()
            .Where(cp => cp.Status == ProgressStatus.Completed)
            .Select(cp => cp.TeacherId)
            .Distinct()
            .CountAsync(cancellationToken);

        var cpdBadgeCompletionRate = totalTeachers > 0 ? (int)((teachersWithCpdBadges * 100.0) / totalTeachers) : 0;

        // Get resource usage stats (placeholder since no Resource entity exists)
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        
        var uploadsThisMonth = subjectId.HasValue
            ? await portfolioRepository.Get()
                .CountAsync(p => p.SubjectId == subjectId.Value && 
                                 p.UploadedAt.Month == currentMonth && 
                                 p.UploadedAt.Year == currentYear, cancellationToken)
            : await portfolioRepository.Get()
                .CountAsync(p => p.UploadedAt.Month == currentMonth && 
                                 p.UploadedAt.Year == currentYear, cancellationToken);

        var resourceUsage = new ResourceUsageStatsDto
        {
            TotalResources = subjectId.HasValue 
                ? await portfolioRepository.Get().CountAsync(p => p.SubjectId == subjectId.Value, cancellationToken)
                : await portfolioRepository.Get().CountAsync(cancellationToken),
            DownloadsThisMonth = 0, // No download tracking
            UploadsThisMonth = uploadsThisMonth,
            MostPopularResource = "N/A"
        };

        // Get top teachers by performance
        var teacherPerformanceData = await (
            from tf in teacherFeedbackRepository.Get()
            join u in userRepository.Get() on tf.TeacherId equals u.ID
            where teacherIds.Contains(tf.TeacherId)
            group tf by new { tf.TeacherId, u.Name, tf.SubjectId } into g
            select new
            {
                TeacherId = g.Key.TeacherId,
                TeacherName = g.Key.Name,
                SubjectId = g.Key.SubjectId,
                FeedbackCount = g.Count(),
                StudentEngagement = g.Select(x => x.StudentId).Distinct().Count()
            })
            .ToListAsync(cancellationToken);

        var cpdBadgesByTeacher = await cpdProgressRepository.Get()
            .Where(cp => teacherIds.Contains(cp.TeacherId) && cp.Status == ProgressStatus.Completed)
            .GroupBy(cp => cp.TeacherId)
            .Select(g => new
            {
                TeacherId = g.Key,
                BadgeCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        var subjectsData = await subjectsRepository.Get()
            .ToDictionaryAsync(s => s.ID, s => s.Name, cancellationToken);

        var topTeachers = teacherPerformanceData
            .Select(tp => new TeacherPerformanceDto
            {
                TeacherName = tp.TeacherName,
                Subject = tp.SubjectId.HasValue && subjectsData.ContainsKey(tp.SubjectId.Value) 
                    ? subjectsData[tp.SubjectId.Value] 
                    : "Unknown",
                CpdBadges = cpdBadgesByTeacher.FirstOrDefault(b => b.TeacherId == tp.TeacherId)?.BadgeCount ?? 0,
                StudentEngagement = tp.StudentEngagement,
                PortfolioReviews = tp.FeedbackCount
            })
            .OrderByDescending(t => t.PortfolioReviews)
            .ThenByDescending(t => t.CpdBadges)
            .Take(5)
            .ToList();

        var result = new SubjectAnalyticsDto
        {
            Subject = subjectName,
            TeacherCount = teacherCount,
            StudentCount = studentCount,
            PortfolioCompletionRate = portfolioCompletionRate,
            CpdBadgeCompletionRate = cpdBadgeCompletionRate,
            ResourceUsage = resourceUsage,
            TopTeachers = topTeachers
        };

        return RequestResult<SubjectAnalyticsDto>.Success(result);
    }
}
