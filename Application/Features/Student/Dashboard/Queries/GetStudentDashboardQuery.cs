using API.Application.Features.Student.Badges.DTOs;
using API.Application.Features.Student.Dashboard.DTOs;
using API.Application.Features.Student.Missions.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.Missions;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Application.Features.Student.Dashboard.Queries;

public record GetStudentDashboardQuery : IRequest<RequestResult<StudentDashboardDto>>;

public class GetStudentDashboardQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<StudentBadges> studentBadgesRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository,
    IRepository<StudentMissionProgress> missionProgressRepository,
    IRepository<Domain.Entities.Missions.Missions> missionsRepository,
    IRepository<PortfolioFiles> portfolioFilesRepository,
    IRepository<TeacherFeedback> feedbackRepository,
    IRepository<StudentChallenges> challengesRepository,
    IRepository<StudentLevels> studentLevelsRepository,
    IRepository<Domain.Entities.General.Subjects> subjectsRepository)
    : RequestHandlerBase<GetStudentDashboardQuery, RequestResult<StudentDashboardDto>>(parameters)
{
    public override async Task<RequestResult<StudentDashboardDto>> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get student info
        var student = await userRepository.Get(x => x.ID == studentId).FirstOrDefaultAsync(cancellationToken);
        if (student == null)
            return RequestResult<StudentDashboardDto>.Failure(ErrorCode.UserNotFound, "Student not found");

        var studentLevel = await studentLevelsRepository.Get(x => x.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);

        var studentInfo = new StudentInfoDto
        {
            Id = student.ID,
            Name = student.Name,
            Email = student.Email,
            Class = "", // TODO: Get from student class entity
            AvatarUrl = "", // TODO: Get avatar URL
            Level = studentLevel?.LevelName.GetDescription() ?? "Digital Scout"
        };

        // Get quick stats
        var totalBadges = await studentBadgesRepository.Get(x => x.StudentId == studentId && x.Status == Status.Approved)
            .CountAsync(cancellationToken);
        var completedMissions = await missionProgressRepository.Get(x => x.StudentId == studentId && x.Status == ProgressStatus.Completed)
            .CountAsync(cancellationToken);
        var portfolioFiles = await portfolioFilesRepository.Get(x => x.StudentId == studentId)
            .CountAsync(cancellationToken);
        var totalPoints = await challengesRepository.Get(x => x.StudentId == studentId)
            .SumAsync(x => x.PointsEarned, cancellationToken);

        var quickStats = new QuickStatsDto
        {
            TotalBadges = totalBadges,
            CompletedMissions = completedMissions,
            PortfolioFiles = portfolioFiles,
            Points = totalPoints
        };

        // Get subject hubs
        var subjects = await subjectsRepository.Get().ToListAsync(cancellationToken);
        var subjectHubs = new List<SubjectCardDto>();

        foreach (var subject in subjects)
        {
            var newFeedbackCount = await feedbackRepository.Get(x => 
                x.StudentId == studentId && 
                x.SubjectId == subject.ID &&
                x.CreatedAt > DateTime.UtcNow.AddDays(-7)) // Last 7 days
                .CountAsync(cancellationToken);

            subjectHubs.Add(new SubjectCardDto
            {
                SubjectId = subject.ID,
                SubjectName = subject.Name,
                Icon = subject.Icon ?? "📚",
                NewFeedbackCount = newFeedbackCount,
                PendingTasksCount = 0 // TODO: Implement if task entity exists
            });
        }

        // Get in-progress missions
        var inProgressMissionIds = await missionProgressRepository
            .Get(x => x.StudentId == studentId && x.Status == ProgressStatus.InProgress)
            .Select(x => x.MissionId)
            .ToListAsync(cancellationToken);

        var inProgressMissions = await missionsRepository.Get(x => inProgressMissionIds.Contains(x.ID))
            .Select(m => new MissionDto
            {
                Id = m.ID,
                Title = m.Title,
                Description = m.Description ?? "",
                Icon = m.Icon ?? "📚",
                Status = "in-progress",
                Progress = 50, // Simplified
                Badge = "",
                Duration = $"{m.EstimatedMinutes} mins",
                Requirements = new()
            })
            .Take(5)
            .ToListAsync(cancellationToken);

        // Get recent badges
        var recentBadgeIds = await studentBadgesRepository
            .Get(x => x.StudentId == studentId && x.Status == Status.Approved)
            .OrderByDescending(x => x.EarnedDate)
            .Take(5)
            .Select(x => x.BadgeId)
            .ToListAsync(cancellationToken);

        var recentBadges = await badgesRepository.Get(x => recentBadgeIds.Contains(x.ID))
            .Select(b => new BadgeDto
            {
                Id = b.ID,
                Name = b.Name,
                Icon = b.Icon ?? "🏆",
                Earned = true,
                EarnDate = DateTime.UtcNow, // Simplified
                Requirement = b.Description ?? "",
                Category = b.Category.ToString()
            })
            .ToListAsync(cancellationToken);

        // Placeholder notifications (no entity exists)
        var notifications = new List<NotificationDto>();

        var dashboard = new StudentDashboardDto
        {
            StudentInfo = studentInfo,
            QuickStats = quickStats,
            SubjectHubs = subjectHubs,
            InProgressMissions = inProgressMissions,
            Notifications = notifications,
            RecentBadges = recentBadges
        };

        return RequestResult<StudentDashboardDto>.Success(dashboard);
    }
}
