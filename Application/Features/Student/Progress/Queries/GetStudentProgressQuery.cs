using API.Application.Features.Student.Progress.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.Missions;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Application.Features.Student.Progress.Queries;

public record GetStudentProgressQuery : IRequest<RequestResult<StudentProgressDto>>;

public class GetStudentProgressQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentBadges> studentBadgesRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository,
    IRepository<StudentMissionProgress> missionProgressRepository,
    IRepository<Domain.Entities.Missions.Missions> missionsRepository,
    IRepository<PortfolioFiles> portfolioFilesRepository,
    IRepository<TeacherFeedback> feedbackRepository,
    IRepository<StudentLevels> studentLevelsRepository,
    IRepository<StudentChallenges> challengesRepository,
    IRepository<Domain.Entities.General.Subjects> subjectsRepository)
    : RequestHandlerBase<GetStudentProgressQuery, RequestResult<StudentProgressDto>>(parameters)
{
    public override async Task<RequestResult<StudentProgressDto>> Handle(GetStudentProgressQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get total points from challenges
        var totalPoints = await challengesRepository.Get(x => x.StudentId == studentId)
            .SumAsync(x => x.PointsEarned, cancellationToken);

        // Get student level
        var studentLevel = await studentLevelsRepository.Get(x => x.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);

        var currentLevel = studentLevel?.LevelName.GetDescription() ?? "Digital Scout";
        
        // Calculate level progress (simple: every 100 points = 1 level)
        var levelProgress = (totalPoints % 100);

        // Get mission progress
        var allMissions = await missionsRepository.Get(x => x.IsEnabled).CountAsync(cancellationToken);
        var progressList = await missionProgressRepository.Get(x => x.StudentId == studentId).ToListAsync(cancellationToken);
        
        var completedMissions = progressList.Count(x => x.Status == ProgressStatus.Completed);
        var inProgressMissions = progressList.Count(x => x.Status == ProgressStatus.InProgress);
        var lockedMissions = allMissions - progressList.Count;

        var missionProgress = new MissionProgressSummaryDto
        {
            TotalMissions = allMissions,
            CompletedMissions = completedMissions,
            InProgressMissions = inProgressMissions,
            LockedMissions = lockedMissions
        };

        // Get badge progress
        var totalBadges = await badgesRepository.Get(x => x.IsActive).CountAsync(cancellationToken);
        var earnedBadges = await studentBadgesRepository.Get(x => x.StudentId == studentId && x.Status == Status.Approved)
            .CountAsync(cancellationToken);

        var badgeProgress = new BadgeProgressDto
        {
            TotalBadges = totalBadges,
            EarnedBadges = earnedBadges,
            Percentage = totalBadges > 0 ? (earnedBadges * 100 / totalBadges) : 0
        };

        // Get subject progress (optimized to avoid N+1 queries)
        var subjects = await subjectsRepository.Get().ToListAsync(cancellationToken);
        var subjectIds = subjects.Select(s => s.ID).ToList();

        var filesCounts = await portfolioFilesRepository.Get(x => x.StudentId == studentId && subjectIds.Contains(x.SubjectId))
            .GroupBy(x => x.SubjectId)
            .Select(g => new { SubjectId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var feedbackCounts = await feedbackRepository.Get(x => x.StudentId == studentId && subjectIds.Contains(x.SubjectId.Value))
            .GroupBy(x => x.SubjectId)
            .Select(g => new { SubjectId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var filesCountMap = filesCounts.ToDictionary(x => x.SubjectId, x => x.Count);
        var feedbackCountMap = feedbackCounts.ToDictionary(x => x.SubjectId, x => x.Count);

        var subjectProgress = subjects.Select(subject =>
        {
            var filesCount = filesCountMap.GetValueOrDefault(subject.ID, 0);
            var feedbackCount = feedbackCountMap.GetValueOrDefault(subject.ID, 0);

            return new SubjectProgressDto
            {
                SubjectId = subject.ID,
                SubjectName = subject.Name,
                FilesUploaded = filesCount,
                FeedbackReceived = feedbackCount,
                BadgesEarned = 0, // TODO: Subject-specific badges
                CompletionPercentage = Math.Min(100, filesCount * 20) // Simple: 5 files = 100%
            };
        }).ToList();

        // Get recent activity logs
        var recentUploads = await portfolioFilesRepository.Get(x => x.StudentId == studentId)
            .OrderByDescending(x => x.UploadedAt)
            .Take(5)
            .Select(x => new ActivityLogDto
            {
                Date = x.UploadedAt,
                ActivityType = "upload",
                Description = $"Uploaded {x.FileName}",
                Icon = "📁"
            })
            .ToListAsync(cancellationToken);

        var recentBadges = await studentBadgesRepository.Get(x => x.StudentId == studentId && x.Status == Status.Approved)
            .OrderByDescending(x => x.EarnedDate)
            .Take(5)
            .Select(x => new ActivityLogDto
            {
                Date = x.EarnedDate,
                ActivityType = "badge_earned",
                Description = "Earned a new badge",
                Icon = "🏆"
            })
            .ToListAsync(cancellationToken);

        var recentActivity = recentUploads.Concat(recentBadges)
            .OrderByDescending(x => x.Date)
            .Take(10)
            .ToList();

        var result = new StudentProgressDto
        {
            TotalPoints = totalPoints,
            CurrentLevel = currentLevel,
            LevelProgress = levelProgress,
            SubjectProgress = subjectProgress,
            MissionProgress = missionProgress,
            BadgeProgress = badgeProgress,
            RecentActivity = recentActivity
        };

        return RequestResult<StudentProgressDto>.Success(result);
    }
}
