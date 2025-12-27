using API.Application.Features.Student.Missions.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Missions.Commands;

public record UpdateMissionProgressCommand(long MissionId, UpdateMissionProgressRequest Request) : IRequest<RequestResult<MissionProgressResponse>>;

public class UpdateMissionProgressCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentMissionProgress> progressRepository,
    IRepository<StudentActivityProgress> activityProgressRepository,
    IRepository<Activities> activitiesRepository,
    IRepository<Domain.Entities.Missions.Missions> missionsRepository,
    IRepository<Domain.Entities.Gamification.StudentBadges> studentBadgesRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository,
    IRepository<Domain.Entities.Gamification.StudentLevels> studentLevelsRepository)
    : RequestHandlerBase<UpdateMissionProgressCommand, RequestResult<MissionProgressResponse>>(parameters)
{
    public override async Task<RequestResult<MissionProgressResponse>> Handle(UpdateMissionProgressCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get or create mission progress
        var missionProgress = await progressRepository.Get(x => x.StudentId == studentId && x.MissionId == request.MissionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (missionProgress == null)
        {
            var totalActivities = await activitiesRepository.Get(x => x.MissionId == request.MissionId).CountAsync(cancellationToken);
            
            missionProgress = new StudentMissionProgress
            {
                StudentId = studentId,
                MissionId = request.MissionId,
                Status = ProgressStatus.InProgress,
                StartedAt = DateTime.UtcNow,
                TotalActivities = totalActivities,
                CompletedActivities = 0
            };
             progressRepository.Add(missionProgress);
        }

        // Update or create activity progress
        var activityProgress = await activityProgressRepository.Get(x => 
            x.StudentId == studentId && 
            x.MissionId == request.MissionId && 
            x.ActivityId == request.Request.ActivityId)
            .FirstOrDefaultAsync(cancellationToken);

        if (request.Request.Completed)
        {
            if (activityProgress == null)
            {
                // Create new activity progress record
                activityProgress = new StudentActivityProgress
                {
                    StudentId = studentId,
                    MissionId = request.MissionId,
                    ActivityId = request.Request.ActivityId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };
                activityProgressRepository.Add(activityProgress);
                missionProgress.CompletedActivities++;
            }
            else if (!activityProgress.IsCompleted)
            {
                // Update existing record to completed (was previously incomplete)
                activityProgress.IsCompleted = true;
                activityProgress.CompletedAt = DateTime.UtcNow;
                activityProgressRepository.Update(activityProgress);
                missionProgress.CompletedActivities++;
            }
            // If already completed, do nothing (prevent duplicate increments)
        }
        else
        {
            // Mark as incomplete
            if (activityProgress != null && activityProgress.IsCompleted)
            {
                activityProgress.IsCompleted = false;
                activityProgress.CompletedAt = null;
                activityProgressRepository.Update(activityProgress);
                
                // Decrement completed activities count
                if (missionProgress.CompletedActivities > 0)
                {
                    missionProgress.CompletedActivities--;
                }
            }
        }

        // Calculate progress percentage
        missionProgress.ProgressPercentage = missionProgress.TotalActivities > 0
            ? (decimal)missionProgress.CompletedActivities / missionProgress.TotalActivities * 100
            : 0;

        // Update mission status based on completion
        if (missionProgress.CompletedActivities >= missionProgress.TotalActivities && missionProgress.TotalActivities > 0)
        {
            missionProgress.Status = ProgressStatus.Completed;
            if (missionProgress.CompletedAt == null)
            {
                missionProgress.CompletedAt = DateTime.UtcNow;
            }
        }
        else if (missionProgress.CompletedActivities > 0)
        {
            missionProgress.Status = ProgressStatus.InProgress;
            missionProgress.CompletedAt = null; // Reset completion date if activities undone
        }
        else
        {
            missionProgress.Status = ProgressStatus.NotStarted;
            missionProgress.CompletedAt = null;
        }

        progressRepository.Update(missionProgress);
        await progressRepository.SaveChangesAsync(cancellationToken);
        
        // Save activity progress changes
        await activityProgressRepository.SaveChangesAsync(cancellationToken);

        // Award badge if mission completed
        Portfolio.DTOs.PortfolioBadgeDto? badgeEarned = null;
        if (missionProgress.Status == ProgressStatus.Completed)
        {
            var mission = await missionsRepository.Get(x => x.ID == request.MissionId).FirstOrDefaultAsync(cancellationToken);
            if (mission != null && mission.BadgeId > 0)
            {
                var existingBadge = await studentBadgesRepository.Get(x => 
                    x.StudentId == studentId && 
                    x.BadgeId == mission.BadgeId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingBadge == null)
                {
                    var badge = await badgesRepository.Get(x => x.ID == mission.BadgeId).FirstOrDefaultAsync(cancellationToken);
                    if (badge != null)
                    {
                        var studentBadge = new Domain.Entities.Gamification.StudentBadges
                        {
                            StudentId = studentId,
                            BadgeId = mission.BadgeId,
                            MissionId = request.MissionId,
                            AutoAwarded = true,
                            EarnedDate = DateTime.UtcNow,
                            Status = Status.Approved
                        };
                        studentBadgesRepository.Add(studentBadge);
                        await studentBadgesRepository.SaveChangesAsync(cancellationToken);

                        // Update student level
                        await UpdateStudentLevelAsync(studentId, cancellationToken);

                        badgeEarned = new Portfolio.DTOs.PortfolioBadgeDto
                        {
                            Id = badge.ID,
                            Name = badge.Name,
                            Description = badge.Description ?? "",
                            Icon = badge.Icon ?? "🏆",
                            Color = badge.Color ?? "#FFD700",
                            EarnedDate = DateTime.UtcNow,
                            Category = badge.Category.ToString()
                        };
                    }
                }
            }
        }

        var response = new MissionProgressResponse
        {
            MissionId = request.MissionId,
            NewProgress = (int)missionProgress.ProgressPercentage,
            Status = missionProgress.Status == ProgressStatus.Completed ? "completed" : "in-progress",
            BadgeEarned = badgeEarned
        };

        return RequestResult<MissionProgressResponse>.Success(response);
    }

    private async Task UpdateStudentLevelAsync(long studentId, CancellationToken cancellationToken)
    {
        // Get or create student level record
        var studentLevel = await studentLevelsRepository.Get(x => x.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);

        // Count total badges earned by this student
        var totalBadges = await studentBadgesRepository.Get(x => x.StudentId == studentId && x.Status == Status.Approved)
            .CountAsync(cancellationToken);

        if (studentLevel == null)
        {
            studentLevel = new StudentLevels
            {
                StudentId = studentId,
                CurrentLevel = 1,
                LevelName = StudentLevelName.DigitalScout,
                BadgesEarned = totalBadges,
                NextLevelBadgeCount = 3,
                LevelIcon = "🌟"
            };
            studentLevelsRepository.Add(studentLevel);
        }
        else
        {
            studentLevel.BadgesEarned = totalBadges;
        }

        // Calculate new level based on badges earned
        // Level thresholds: Scout (0-2), Explorer (3-5), Champion (6-7), Leader (8+)
        var (newLevel, newLevelName, nextThreshold, icon) = totalBadges switch
        {
            >= 8 => (4, StudentLevelName.Leader, (int?)null, "👑"),
            >= 6 => (3, StudentLevelName.Champion, 8, "🏆"),
            >= 3 => (2, StudentLevelName.Explorer, 6, "🚀"),
            _ => (1, StudentLevelName.DigitalScout, 3, "🌟")
        };

        // Check if level up occurred
        if (newLevel > studentLevel.CurrentLevel)
        {
            studentLevel.LastLevelUpDate = DateTime.UtcNow;
        }

        studentLevel.CurrentLevel = newLevel;
        studentLevel.LevelName = newLevelName;
        studentLevel.NextLevelBadgeCount = nextThreshold;
        studentLevel.LevelIcon = icon;

        studentLevelsRepository.Update(studentLevel);
        await studentLevelsRepository.SaveChangesAsync(cancellationToken);
    }
}
