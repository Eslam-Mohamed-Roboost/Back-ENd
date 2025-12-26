using API.Application.Features.Student.Progress.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.Missions;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Progress.Queries;

public record GetLearningHoursQuery : IRequest<RequestResult<LearningHoursSummaryDto>>;

public class GetLearningHoursQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentMissionProgress> missionProgressRepository,
    IRepository<Domain.Entities.Missions.Missions> missionsRepository,
    IRepository<StudentChallenges> challengesRepository,
    IRepository<PortfolioFiles> portfolioFilesRepository,
    IRepository<StudentBadges> studentBadgesRepository)
    : RequestHandlerBase<GetLearningHoursQuery, RequestResult<LearningHoursSummaryDto>>(parameters)
{
    public override async Task<RequestResult<LearningHoursSummaryDto>> Handle(GetLearningHoursQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var now = DateTime.UtcNow;
        var weekStart = now.AddDays(-(int)now.DayOfWeek);
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var activities = new List<LearningHoursEntryDto>();

        // 1. Mission completions - use estimated minutes from missions
        var completedMissions = await (from mp in missionProgressRepository.Get()
                                       join m in missionsRepository.Get() on mp.MissionId equals m.ID
                                       where mp.StudentId == studentId && mp.Status == ProgressStatus.Completed && mp.CompletedAt.HasValue
                                       select new { mp.ID, mp.MissionId, mp.CompletedAt!.Value, m.EstimatedMinutes,m.CreatedAt })
            .ToListAsync(cancellationToken);

        foreach (var mission in completedMissions)
        {
            var hours = (decimal)mission.EstimatedMinutes / 60m;
            activities.Add(new LearningHoursEntryDto
            {
                Id = mission.ID,
                ActivityType = "Mission",
                ActivityId = mission.MissionId,
                HoursEarned = hours,
                EarnedDate = mission.CreatedAt
            });
        }

        // 2. Challenge completions - estimate 0.5 hours per challenge
        var completedChallenges = await challengesRepository.Get(x =>
            x.StudentId == studentId &&
            x.Status == ProgressStatus.Completed &&
            x.CompletedAt.HasValue)
            .Select(x => new { x.ID, x.ChallengeId, x.CompletedAt!.Value,x.CompletedAt })
            .ToListAsync(cancellationToken);

        foreach (var challenge in completedChallenges)
        {
            activities.Add(new LearningHoursEntryDto
            {
                Id = challenge.ID,
                ActivityType = "Challenge",
                ActivityId = challenge.ChallengeId,
                HoursEarned = 0.5m, // Estimate 30 minutes per challenge
                EarnedDate = challenge.CompletedAt.GetValueOrDefault()
            });
        }

        // 3. Portfolio uploads - estimate 0.25 hours per upload
        var portfolioUploads = await portfolioFilesRepository.Get(x =>
            x.StudentId == studentId)
            .Select(x => new { x.ID, x.SubjectId, x.UploadedAt })
            .ToListAsync(cancellationToken);

        foreach (var upload in portfolioUploads)
        {
            activities.Add(new LearningHoursEntryDto
            {
                Id = upload.ID,
                ActivityType = "Portfolio",
                ActivityId = upload.SubjectId,
                HoursEarned = 0.25m, // Estimate 15 minutes per upload
                EarnedDate = upload.UploadedAt
            });
        }

        // 4. Badge awards - estimate 0.1 hours per badge (recognition time)
        var badgeAwards = await studentBadgesRepository.Get(x =>
            x.StudentId == studentId &&
            x.Status == Status.Approved)
            .Select(x => new { x.ID, x.BadgeId, x.EarnedDate })
            .ToListAsync(cancellationToken);

        foreach (var badge in badgeAwards)
        {
            activities.Add(new LearningHoursEntryDto
            {
                Id = badge.ID,
                ActivityType = "Completion",
                ActivityId = badge.BadgeId,
                HoursEarned = 0.1m, // Estimate 6 minutes per badge
                EarnedDate = badge.EarnedDate
            });
        }

        // Calculate totals
        var totalHours = activities.Sum(a => a.HoursEarned);
        var thisWeekHours = activities.Where(a => a.EarnedDate >= weekStart).Sum(a => a.HoursEarned);
        var thisMonthHours = activities.Where(a => a.EarnedDate >= monthStart).Sum(a => a.HoursEarned);

        // Group by activity type
        var byActivityType = activities
            .GroupBy(a => a.ActivityType)
            .Select(g => new ActivityHoursBreakdownDto
            {
                ActivityType = g.Key,
                TotalHours = g.Sum(a => a.HoursEarned),
                Count = g.Count()
            })
            .ToList();

        // Get recent activities (last 20)
        var recentActivities = activities
            .OrderByDescending(a => a.EarnedDate)
            .Take(20)
            .ToList();

        var summary = new LearningHoursSummaryDto
        {
            TotalHours = totalHours,
            ThisWeekHours = thisWeekHours,
            ThisMonthHours = thisMonthHours,
            ByActivityType = byActivityType,
            RecentActivities = recentActivities
        };

        return RequestResult<LearningHoursSummaryDto>.Success(summary);
    }
}

