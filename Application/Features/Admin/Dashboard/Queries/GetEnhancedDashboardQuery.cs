using API.Application.Features.Admin.Dashboard.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Dashboard.Queries;

public record GetEnhancedDashboardQuery : IRequest<RequestResult<EnhancedDashboardDto>>;

public class GetEnhancedDashboardQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository,
    IRepository<Domain.Entities.Teacher.TeacherBadgeSubmissions> badgeSubmissionRepository)
    : RequestHandlerBase<GetEnhancedDashboardQuery, RequestResult<EnhancedDashboardDto>>(parameters)
{
    public override async Task<RequestResult<EnhancedDashboardDto>> Handle(
        GetEnhancedDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
        var lastWeekStart = startOfWeek.AddDays(-7);

        // ============================================
        // STUDENT ACHIEVEMENT METRICS
        // ============================================
        var students = userRepository.Get(u => u.Role == ApplicationRole.Student);
        var totalStudents = await students.CountAsync(cancellationToken);
        
        // Mock grade distribution (would come from student profile data)
        var grade6Count = totalStudents / 2;
        var grade7Count = totalStudents - grade6Count;
        
        // Mock metrics - in production these would come from actual portfolio/progress data
        var digitalCitizenshipProgress = 68m;
        var portfolioQualityScore = 7.2m;
        var atRiskCount = 23; // Students with <50% completion or quality <5.0
        var topPerformersCount = 45; // Students with >90% completion and quality >8.0

        var studentAchievement = new StudentAchievementMetrics
        {
            TotalStudents = totalStudents,
            Grade6Count = grade6Count,
            Grade7Count = grade7Count,
            DigitalCitizenshipProgress = digitalCitizenshipProgress,
            PortfolioQualityScore = portfolioQualityScore,
            AtRiskCount = atRiskCount,
            TopPerformersCount = topPerformersCount
        };

        // ============================================
        // TEACHER CPD METRICS
        // ============================================
        var teachers = userRepository.Get(u => u.Role == ApplicationRole.Teacher);
        var totalTeachers = await teachers.CountAsync(cancellationToken);

        // Active teachers this month (have CPD activity)
        var activeTeacherIds = await cpdProgressRepository.Get()
            .Where(cp => cp.LastAccessedAt >= startOfMonth || cp.CompletedAt >= startOfMonth)
            .Select(cp => cp.TeacherId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var activeTeachers = activeTeacherIds.Count;

        // CPD hours this month
        var cpdHoursThisMonth = await cpdProgressRepository.Get()
            .Where(cp => cp.CompletedAt >= startOfMonth && cp.Status == ProgressStatus.Completed)
            .SumAsync(cp => cp.HoursEarned ?? 0, cancellationToken);

        // Badge completion rate (teachers with ≥3 badges)
        var teachersWithBadges = await badgeSubmissionRepository.Get()
            .Where(bs => bs.Status == SubmissionStatus.Approved)
            .GroupBy(bs => bs.TeacherId)
            .Where(g => g.Count() >= 3)
            .CountAsync(cancellationToken);
        var badgeCompletionRate = activeTeachers > 0 ? (decimal)teachersWithBadges / activeTeachers * 100 : 0;

        // Top performer
        var topPerformer = await cpdProgressRepository.Get()
            .Where(cp => cp.CompletedAt >= startOfMonth && cp.Status == ProgressStatus.Completed)
            .GroupBy(cp => cp.TeacherId)
            .Select(g => new { TeacherId = g.Key, Hours = g.Sum(cp => cp.HoursEarned ?? 0) })
            .OrderByDescending(x => x.Hours)
            .FirstOrDefaultAsync(cancellationToken);

        var topPerformerName = "N/A";
        var topPerformerHours = 0m;
        if (topPerformer != null)
        {
            var teacher = await teachers.FirstOrDefaultAsync(t => t.ID == topPerformer.TeacherId, cancellationToken);
            topPerformerName = teacher?.Name ?? "Unknown";
            topPerformerHours = topPerformer.Hours;
        }

        var teacherCPD = new TeacherCPDMetrics
        {
            TotalTeachers = totalTeachers,
            ActiveTeachers = activeTeachers,
            CPDHoursThisMonth = cpdHoursThisMonth,
            TargetHoursThisMonth = 24m,
            BadgeCompletionRate = badgeCompletionRate,
            ResourceDownloads = 797, // Mock - would come from resource tracking
            TopPerformerName = topPerformerName,
            TopPerformerHours = topPerformerHours
        };

        // ============================================
        // ADEK COMPLIANCE METRICS
        // ============================================
        var totalBadges = await badgeSubmissionRepository.Get()
            .Where(bs => bs.Status == SubmissionStatus.Approved)
            .CountAsync(cancellationToken);

        var studentsWithPortfolios = await students
            .Where(s => s.Badges.Any())
            .CountAsync(cancellationToken);
        var portfolioCompletionPercentage = totalStudents > 0 
            ? (decimal)studentsWithPortfolios / totalStudents * 100 
            : 0;

        var teachersWithCPD = await cpdProgressRepository.Get()
            .Where(cp => cp.Status == ProgressStatus.Completed)
            .Select(cp => cp.TeacherId)
            .Distinct()
            .CountAsync(cancellationToken);
        var cpdDocumentationPercentage = activeTeachers > 0 
            ? (decimal)teachersWithCPD / activeTeachers * 100 
            : 0;

        var pendingReviewCount = await badgeSubmissionRepository.Get()
            .CountAsync(bs => bs.Status == SubmissionStatus.Pending, cancellationToken);

        var nextDeadline = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc); // ADEK deadline
        var nextDeadlineDate = new DateTime(nextDeadline.Year, nextDeadline.Month, nextDeadline.Day, 0, 0, 0, DateTimeKind.Utc);
        var nowDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        var daysUntilDeadline = (nextDeadlineDate - nowDate).Days;

        var adekCompliance = new ADEKComplianceMetrics
        {
            TotalEvidenceItems = totalBadges + studentsWithPortfolios,
            PortfolioCompletionPercentage = portfolioCompletionPercentage,
            CPDDocumentationPercentage = cpdDocumentationPercentage,
            PendingReviewCount = pendingReviewCount,
            NextDeadline = nextDeadline,
            DaysUntilDeadline = daysUntilDeadline
        };

        // ============================================
        // PLATFORM ENGAGEMENT METRICS
        // ============================================
        // Mock data - in production would come from activity logs
        var totalUsers = totalStudents + totalTeachers;
        var dailyActiveUsers = (int)(totalUsers * 0.74m); // 74% engagement
        var engagementPercentage = totalUsers > 0 ? (decimal)dailyActiveUsers / totalUsers * 100 : 0;

        // Weekly trend
        var thisWeekActive = dailyActiveUsers;
        var lastWeekActive = (int)(dailyActiveUsers * 0.88m); // 12% increase
        var weeklyTrendPercentage = lastWeekActive > 0 
            ? ((decimal)(thisWeekActive - lastWeekActive) / lastWeekActive * 100) 
            : 0;

        var platformEngagement = new PlatformEngagementMetrics
        {
            DailyActiveUsers = dailyActiveUsers,
            TotalUsers = totalUsers,
            EngagementPercentage = engagementPercentage,
            PeakUsageTime = "10:00 AM - 11:30 AM",
            MostAccessedResource = "DC Module 3",
            OneNoteAdoptionRate = 95m,
            BadgeAdoptionRate = 82m,
            WeeklyTrendPercentage = weeklyTrendPercentage
        };

        // ============================================
        // AI INSIGHTS
        // ============================================
        var aiInsights = new List<AIInsightDto>();

        // Alert 1: At-risk students
        if (atRiskCount > 0)
        {
            aiInsights.Add(new AIInsightDto
            {
                Type = "warning",
                Icon = "⚠️",
                Message = $"{atRiskCount} students at risk of portfolio incompletion",
                Count = atRiskCount
            });
        }

        // Alert 2: CPD participation trend
        var cpdTrendPercentage = -15m; // Mock - would calculate from actual data
        if (cpdTrendPercentage < 0)
        {
            aiInsights.Add(new AIInsightDto
            {
                Type = "alert",
                Icon = "📉",
                Message = $"Teacher CPD participation down {Math.Abs(cpdTrendPercentage)}% this week",
                Count = null
            });
        }

        // Alert 3: Pending badges
        if (pendingReviewCount > 0)
        {
            aiInsights.Add(new AIInsightDto
            {
                Type = "info",
                Icon = "🏅",
                Message = $"{pendingReviewCount} new badges pending approval",
                Count = pendingReviewCount,
                ActionLink = "/admin/badges"
            });
        }

        // Alert 4: ADEK deadline
        aiInsights.Add(new AIInsightDto
        {
            Type = "info",
            Icon = "📁",
            Message = $"ADEK Evidence: {adekCompliance.TotalEvidenceItems} items | Report due in {daysUntilDeadline} days",
            Count = adekCompliance.TotalEvidenceItems
        });

        var result = new EnhancedDashboardDto
        {
            StudentAchievement = studentAchievement,
            TeacherCPD = teacherCPD,
            ADEKCompliance = adekCompliance,
            PlatformEngagement = platformEngagement,
            AIInsights = aiInsights
        };

        return RequestResult<EnhancedDashboardDto>.Success(result);
    }
}

