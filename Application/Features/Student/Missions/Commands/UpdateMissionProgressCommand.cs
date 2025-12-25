using API.Application.Events;
using API.Application.Features.Student.Missions.DTOs;
using API.Application.Services;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using DotNetCore.CAP;
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
    IHoursTrackingService hoursTrackingService,
    INotificationService notificationService,
    ICapPublisher? eventPublisher = null)
    : RequestHandlerBase<UpdateMissionProgressCommand, RequestResult<MissionProgressResponse>>(parameters)
{
    public override async Task<RequestResult<MissionProgressResponse>> Handle(UpdateMissionProgressCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get mission to check deadline and prerequisites
        var mission = await missionsRepository
            .Get(m => m.ID == request.MissionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (mission == null)
        {
            return RequestResult<MissionProgressResponse>.Failure(ErrorCode.NotFound, "Mission not found");
        }

        // Check deadline
        if (mission.Deadline.HasValue && DateTime.UtcNow > mission.Deadline.Value)
        {
            return RequestResult<MissionProgressResponse>.Failure(
                ErrorCode.ValidationError, 
                "Mission deadline has passed. Submissions are no longer accepted.");
        }

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

        if (activityProgress == null && request.Request.Completed)
        {
            activityProgress = new StudentActivityProgress
            {
                StudentId = studentId,
                MissionId = request.MissionId,
                ActivityId = request.Request.ActivityId,
               // Status = ProgressStatus.Completed,
                CompletedAt = DateTime.UtcNow
            };
            activityProgressRepository.Add(activityProgress);
            missionProgress.CompletedActivities++;
        }
        else if (activityProgress != null && request.Request.Completed)
        {
           // activityProgress.Status = ProgressStatus.Completed;
            activityProgress.CompletedAt = DateTime.UtcNow;
             activityProgressRepository.Update(activityProgress);
        }

        // Calculate progress percentage
        missionProgress.ProgressPercentage = missionProgress.TotalActivities > 0
            ? (decimal)missionProgress.CompletedActivities / missionProgress.TotalActivities * 100
            : 0;

        // Check if mission is completed
        if (missionProgress.CompletedActivities >= missionProgress.TotalActivities)
        {
            missionProgress.Status = ProgressStatus.Completed;
            missionProgress.CompletedAt = DateTime.UtcNow;
        }

         progressRepository.Update(missionProgress);

        // Award badge and hours if mission completed
        Portfolio.DTOs.PortfolioBadgeDto? badgeEarned = null;
        decimal hoursAwarded = 0;
        
        if (missionProgress.Status == ProgressStatus.Completed)
        {
            var completedMission = await missionsRepository.Get(x => x.ID == request.MissionId).FirstOrDefaultAsync(cancellationToken);
            if (completedMission != null && completedMission.BadgeId > 0)
            {
                var existingBadge = await studentBadgesRepository.Get(x => 
                    x.StudentId == studentId && 
                    x.BadgeId == completedMission.BadgeId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingBadge == null)
                {
                    var badge = await badgesRepository.Get(x => x.ID == completedMission.BadgeId).FirstOrDefaultAsync(cancellationToken);
                    if (badge != null)
                    {
                        var studentBadge = new Domain.Entities.Gamification.StudentBadges
                        {
                            StudentId = studentId,
                            BadgeId = completedMission.BadgeId,
                            MissionId = request.MissionId,
                            AutoAwarded = true,
                            EarnedDate = DateTime.UtcNow,
                            Status = Status.Approved
                        };
                         studentBadgesRepository.Add(studentBadge);

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
                
                // Record learning hours
                hoursAwarded = await hoursTrackingService.RecordLearningHoursAsync(
                    studentId,
                    ActivityLogType.Completion,
                    request.MissionId,
                    completedMission.HoursAwarded,
                    cancellationToken);
                
                // Send notifications
                await notificationService.SendMissionCompletedNotificationAsync(
                    studentId,
                    request.MissionId,
                    completedMission.Title,
                    cancellationToken);
                
                if (badgeEarned != null)
                {
                    await notificationService.SendBadgeEarnedNotificationAsync(
                        studentId,
                        false,
                        completedMission.BadgeId,
                        badgeEarned.Name,
                        cancellationToken);
                }
                
                if (hoursAwarded > 0)
                {
                    await notificationService.SendHoursAwardedNotificationAsync(
                        studentId,
                        false,
                        hoursAwarded,
                        completedMission.Title,
                        cancellationToken);
                }
                
                // Publish mission completed event (if CAP is configured)
                if (eventPublisher != null)
                {
                    var completionTime = missionProgress.StartedAt.HasValue 
                        ? DateTime.UtcNow - missionProgress.StartedAt.Value 
                        : TimeSpan.Zero;
                        
                    await eventPublisher.PublishAsync("mission.completed", new MissionCompletedEvent
                    {
                        StudentId = studentId,
                        MissionId = request.MissionId,
                        MissionTitle = completedMission.Title,
                        BadgeId = completedMission.BadgeId,
                        HoursAwarded = completedMission.HoursAwarded,
                        CompletedAt = DateTime.UtcNow,
                        CompletionTime = completionTime
                    }, cancellationToken: cancellationToken);
                }
            }
        }

        var response = new MissionProgressResponse
        {
            MissionId = request.MissionId,
            NewProgress = (int)missionProgress.ProgressPercentage,
            Status = missionProgress.Status == ProgressStatus.Completed ? "completed" : "in-progress",
            BadgeEarned = badgeEarned,
            HoursEarned = hoursAwarded
        };

        return RequestResult<MissionProgressResponse>.Success(response);
    }
}
