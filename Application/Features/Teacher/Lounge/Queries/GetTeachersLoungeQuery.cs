using API.Application.Features.Teacher.Lounge.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Lounge.Queries;

public record GetTeachersLoungeQuery : IRequest<RequestResult<TeachersLoungeDto>>;

public class GetTeachersLoungeQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<TeacherCpdProgress> cpdProgressRepository,
    IRepository<TeacherBadgeSubmissions> badgeSubmissionsRepository,
    IRepository<TeacherClassAssignments> classAssignmentsRepository)
    : RequestHandlerBase<GetTeachersLoungeQuery, RequestResult<TeachersLoungeDto>>(parameters)
{
    public override async Task<RequestResult<TeachersLoungeDto>> Handle(GetTeachersLoungeQuery request, CancellationToken cancellationToken)
    {
        var currentTeacherId = _userState.UserID;
        var now = DateTime.UtcNow;
        var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1, 0, 0, 0), DateTimeKind.Utc);
        var startOfLastMonth = DateTime.SpecifyKind(startOfMonth.AddMonths(-1), DateTimeKind.Utc);
        var endOfLastMonth = DateTime.SpecifyKind(startOfMonth.AddDays(-1), DateTimeKind.Utc);

        // Get all teachers
        var teachers = await userRepository.Get(u => u.Role == ApplicationRole.Teacher)
            .Select(u => new { u.ID, u.Name })
            .ToListAsync(cancellationToken);

        var teacherIds = teachers.Select(t => t.ID).ToList();
        var teacherMap = teachers.ToDictionary(t => t.ID, t => t.Name);

        // ============================================
        // CPD LEADERBOARD (This Month)
        // ============================================
        var cpdLeadersData = await cpdProgressRepository.Get()
            .Where(cp => teacherIds.Contains(cp.TeacherId) &&
                        cp.CompletedAt >= startOfMonth &&
                        cp.Status == ProgressStatus.Completed)
            .GroupBy(cp => cp.TeacherId)
            .Select(g => new
            {
                TeacherId = g.Key,
                TotalHours = g.Sum(cp => cp.HoursEarned ?? 0)
            })
            .OrderByDescending(x => x.TotalHours)
            .Take(10)
            .ToListAsync(cancellationToken);

        var cpdLeaders = new List<LeaderboardEntryDto>();
        for (int i = 0; i < cpdLeadersData.Count; i++)
        {
            var item = cpdLeadersData[i];
            var department = await GetDepartmentForTeacher(item.TeacherId, classAssignmentsRepository, cancellationToken);
            cpdLeaders.Add(new LeaderboardEntryDto
            {
                Rank = i + 1,
                TeacherId = item.TeacherId,
                Name = teacherMap.GetValueOrDefault(item.TeacherId) ?? "Unknown",
                Value = $"{item.TotalHours:F1} hours",
                Subtitle = department
            });
        }

        // Get current user's CPD rank
        var currentUserCpdHours = await cpdProgressRepository.Get()
            .Where(cp => cp.TeacherId == currentTeacherId &&
                        cp.CompletedAt >= startOfMonth &&
                        cp.Status == ProgressStatus.Completed)
            .SumAsync(cp => cp.HoursEarned ?? 0, cancellationToken);

        var currentUserCpdRank = await cpdProgressRepository.Get()
            .Where(cp => teacherIds.Contains(cp.TeacherId) &&
                        cp.CompletedAt >= startOfMonth &&
                        cp.Status == ProgressStatus.Completed)
            .GroupBy(cp => cp.TeacherId)
            .Select(g => new { TeacherId = g.Key, TotalHours = g.Sum(cp => cp.HoursEarned ?? 0) })
            .Where(x => x.TotalHours > currentUserCpdHours)
            .CountAsync(cancellationToken) + 1;

        var currentUserCpdRankDto = new CurrentUserRankDto
        {
            Rank = currentUserCpdRank,
            Value = $"{currentUserCpdHours:F1} hours"
        };

        // ============================================
        // BADGE LEADERBOARD (This Month)
        // ============================================
        var badgeLeadersData = await badgeSubmissionsRepository.Get()
            .Where(bs => teacherIds.Contains(bs.TeacherId) &&
                        bs.Status == SubmissionStatus.Approved &&
                        bs.CreatedAt >= startOfMonth)
            .GroupBy(bs => bs.TeacherId)
            .Select(g => new
            {
                TeacherId = g.Key,
                BadgeCount = g.Count()
            })
            .OrderByDescending(x => x.BadgeCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        var badgeLeaders = new List<LeaderboardEntryDto>();
        for (int i = 0; i < badgeLeadersData.Count; i++)
        {
            var item = badgeLeadersData[i];
            var department = await GetDepartmentForTeacher(item.TeacherId, classAssignmentsRepository, cancellationToken);
            badgeLeaders.Add(new LeaderboardEntryDto
            {
                Rank = i + 1,
                TeacherId = item.TeacherId,
                Name = teacherMap.GetValueOrDefault(item.TeacherId) ?? "Unknown",
                Value = $"{item.BadgeCount} badges",
                Subtitle = department
            });
        }

        // Get current user's badge rank
        var currentUserBadgeCount = await badgeSubmissionsRepository.Get()
            .Where(bs => bs.TeacherId == currentTeacherId &&
                        bs.Status == SubmissionStatus.Approved &&
                        bs.CreatedAt >= startOfMonth)
            .CountAsync(cancellationToken);

        var currentUserBadgeRank = await badgeSubmissionsRepository.Get()
            .Where(bs => teacherIds.Contains(bs.TeacherId) &&
                        bs.Status == SubmissionStatus.Approved &&
                        bs.CreatedAt >= startOfMonth)
            .GroupBy(bs => bs.TeacherId)
            .Select(g => new { TeacherId = g.Key, BadgeCount = g.Count() })
            .Where(x => x.BadgeCount > currentUserBadgeCount)
            .CountAsync(cancellationToken) + 1;

        var currentUserBadgeRankDto = new CurrentUserRankDto
        {
            Rank = currentUserBadgeRank,
            Value = $"{currentUserBadgeCount} badges"
        };

        // ============================================
        // MONTHLY STATS
        // ============================================
        // Total CPD Hours This Month
        var totalCpdHoursThisMonth = await cpdProgressRepository.Get()
            .Where(cp => cp.CompletedAt >= startOfMonth &&
                        cp.Status == ProgressStatus.Completed)
            .SumAsync(cp => cp.HoursEarned ?? 0, cancellationToken);

        // Total CPD Hours Last Month
        var totalCpdHoursLastMonth = await cpdProgressRepository.Get()
            .Where(cp => cp.CompletedAt >= startOfLastMonth &&
                        cp.CompletedAt <= endOfLastMonth &&
                        cp.Status == ProgressStatus.Completed)
            .SumAsync(cp => cp.HoursEarned ?? 0, cancellationToken);

        var cpdHoursChangePercent = totalCpdHoursLastMonth > 0
            ? ((totalCpdHoursThisMonth - totalCpdHoursLastMonth) / totalCpdHoursLastMonth) * 100
            : 0;

        // Badges Awarded This Month
        var badgesAwardedThisMonth = await badgeSubmissionsRepository.Get()
            .Where(bs => bs.Status == SubmissionStatus.Approved &&
                        bs.CreatedAt >= startOfMonth)
            .CountAsync(cancellationToken);

        // Badges Awarded Last Month
        var badgesAwardedLastMonth = await badgeSubmissionsRepository.Get()
            .Where(bs => bs.Status == SubmissionStatus.Approved &&
                        bs.CreatedAt >= startOfLastMonth &&
                        bs.CreatedAt <= endOfLastMonth)
            .CountAsync(cancellationToken);

        var badgesChangePercent = badgesAwardedLastMonth > 0
            ? ((badgesAwardedThisMonth - badgesAwardedLastMonth) / (decimal)badgesAwardedLastMonth) * 100
            : 0;

        // Active Teachers This Month (have CPD activity or badge submissions)
        var activeTeacherIdsThisMonth = await cpdProgressRepository.Get()
            .Where(cp => cp.LastAccessedAt >= startOfMonth || cp.CompletedAt >= startOfMonth)
            .Select(cp => cp.TeacherId)
            .Distinct()
            .Union(
                badgeSubmissionsRepository.Get()
                    .Where(bs => bs.CreatedAt >= startOfMonth)
                    .Select(bs => bs.TeacherId)
                    .Distinct()
            )
            .Distinct()
            .CountAsync(cancellationToken);

        // Active Teachers Last Month
        var activeTeacherIdsLastMonth = await cpdProgressRepository.Get()
            .Where(cp => (cp.LastAccessedAt >= startOfLastMonth && cp.LastAccessedAt <= endOfLastMonth) ||
                        (cp.CompletedAt >= startOfLastMonth && cp.CompletedAt <= endOfLastMonth))
            .Select(cp => cp.TeacherId)
            .Distinct()
            .Union(
                badgeSubmissionsRepository.Get()
                    .Where(bs => bs.CreatedAt >= startOfLastMonth && bs.CreatedAt <= endOfLastMonth)
                    .Select(bs => bs.TeacherId)
                    .Distinct()
            )
            .Distinct()
            .CountAsync(cancellationToken);

        var activeTeachersChangePercent = activeTeacherIdsLastMonth > 0
            ? ((activeTeacherIdsThisMonth - activeTeacherIdsLastMonth) / (decimal)activeTeacherIdsLastMonth) * 100
            : 0;

        // Engagement Rate (percentage of teachers who completed at least one CPD module this month)
        var totalTeachers = teacherIds.Count;
        var engagedTeachers = await cpdProgressRepository.Get()
            .Where(cp => cp.CompletedAt >= startOfMonth &&
                        cp.Status == ProgressStatus.Completed)
            .Select(cp => cp.TeacherId)
            .Distinct()
            .CountAsync(cancellationToken);

        var engagementRate = totalTeachers > 0 ? (engagedTeachers / (decimal)totalTeachers) * 100 : 0;

        // Engagement Rate Last Month
        var engagedTeachersLastMonth = await cpdProgressRepository.Get()
            .Where(cp => cp.CompletedAt >= startOfLastMonth &&
                        cp.CompletedAt <= endOfLastMonth &&
                        cp.Status == ProgressStatus.Completed)
            .Select(cp => cp.TeacherId)
            .Distinct()
            .CountAsync(cancellationToken);

        var engagementRateLastMonth = totalTeachers > 0 ? (engagedTeachersLastMonth / (decimal)totalTeachers) * 100 : 0;

        var engagementChangePercent = engagementRateLastMonth > 0
            ? ((engagementRate - engagementRateLastMonth) / engagementRateLastMonth) * 100
            : 0;

        var stats = new TeachersLoungeStatsDto
        {
            TotalCpdHours = totalCpdHoursThisMonth,
            CpdHoursChangePercent = cpdHoursChangePercent,
            BadgesAwarded = badgesAwardedThisMonth,
            BadgesChangePercent = badgesChangePercent,
            ActiveTeachers = activeTeacherIdsThisMonth,
            ActiveTeachersChangePercent = activeTeachersChangePercent,
            EngagementRate = engagementRate,
            EngagementChangePercent = engagementChangePercent
        };

        var result = new TeachersLoungeDto
        {
            CpdLeaders = cpdLeaders,
            CurrentUserCpdRank = currentUserCpdRankDto,
            BadgeLeaders = badgeLeaders,
            CurrentUserBadgeRank = currentUserBadgeRankDto,
            Stats = stats
        };

        return RequestResult<TeachersLoungeDto>.Success(result);
    }

    private static async Task<string> GetDepartmentForTeacher(
        long teacherId,
        IRepository<TeacherClassAssignments> classAssignmentsRepository,
        CancellationToken cancellationToken)
    {
        // Get first subject assigned to teacher as department
        var assignment = await classAssignmentsRepository.Get(a => a.TeacherId == teacherId)
            .FirstOrDefaultAsync(cancellationToken);

        // For now, return a generic department name
        // In a real system, you might have a Department table
        return assignment != null ? "Active Teacher" : "No Assignments";
    }
}

