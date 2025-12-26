using API.Application.Features.Student.Portfolio.DTOs;
using API.Application.Features.Teacher.Missions.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Missions.Commands;

public record UpdateTeacherMissionProgressCommand(long MissionId, UpdateTeacherMissionProgressRequest Request) : IRequest<RequestResult<TeacherMissionProgressResponse>>;

public class UpdateTeacherMissionProgressCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherMissionProgress> progressRepository,
    IRepository<TeacherActivityProgress> activityProgressRepository,
    IRepository<TeacherActivities> activitiesRepository,
    IRepository<TeacherMissions> missionsRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<UpdateTeacherMissionProgressCommand, RequestResult<TeacherMissionProgressResponse>>(parameters)
{
    public override async Task<RequestResult<TeacherMissionProgressResponse>> Handle(UpdateTeacherMissionProgressCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get or create mission progress
        var missionProgress = await progressRepository.Get(x => x.TeacherId == teacherId && x.MissionId == request.MissionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (missionProgress == null)
        {
            var totalActivities = await activitiesRepository.Get(x => x.MissionId == request.MissionId).CountAsync(cancellationToken);
            
            missionProgress = new TeacherMissionProgress
            {
                TeacherId = teacherId,
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
            x.TeacherId == teacherId && 
            x.MissionId == request.MissionId && 
            x.ActivityId == request.Request.ActivityId)
            .FirstOrDefaultAsync(cancellationToken);

        if (activityProgress == null && request.Request.Completed)
        {
            activityProgress = new TeacherActivityProgress
            {
                TeacherId = teacherId,
                MissionId = request.MissionId,
                ActivityId = request.Request.ActivityId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            };
            activityProgressRepository.Add(activityProgress);
            missionProgress.CompletedActivities++;
        }
        else if (activityProgress != null)
        {
            if (request.Request.Completed && !activityProgress.IsCompleted)
            {
                activityProgress.IsCompleted = true;
                activityProgress.CompletedAt = DateTime.UtcNow;
                activityProgressRepository.Update(activityProgress);
                missionProgress.CompletedActivities++;
            }
            else if (!request.Request.Completed && activityProgress.IsCompleted)
            {
                activityProgress.IsCompleted = false;
                activityProgress.CompletedAt = null;
                activityProgressRepository.Update(activityProgress);
                missionProgress.CompletedActivities = Math.Max(0, missionProgress.CompletedActivities - 1);
            }
        }

        // Calculate progress percentage
        missionProgress.ProgressPercentage = missionProgress.TotalActivities > 0
            ? (decimal)missionProgress.CompletedActivities / missionProgress.TotalActivities * 100
            : 0;

        // Check if mission is completed
        if (missionProgress.CompletedActivities >= missionProgress.TotalActivities && missionProgress.TotalActivities > 0)
        {
            missionProgress.Status = ProgressStatus.Completed;
            missionProgress.CompletedAt = DateTime.UtcNow;
        }
        else if (missionProgress.Status == ProgressStatus.Completed && missionProgress.CompletedActivities < missionProgress.TotalActivities)
        {
            missionProgress.Status = ProgressStatus.InProgress;
            missionProgress.CompletedAt = null;
        }

        progressRepository.Update(missionProgress);
        await progressRepository.SaveChangesAsync(cancellationToken);
        await activityProgressRepository.SaveChangesAsync(cancellationToken);

        // Award badge if mission completed (for teachers, we might use TeacherBadgeSubmissions instead)
        PortfolioBadgeDto? badgeEarned = null;
        if (missionProgress.Status == ProgressStatus.Completed)
        {
            var mission = await missionsRepository.Get(x => x.ID == request.MissionId).FirstOrDefaultAsync(cancellationToken);
            if (mission != null && mission.BadgeId > 0)
            {
                var badge = await badgesRepository.Get(x => x.ID == mission.BadgeId).FirstOrDefaultAsync(cancellationToken);
                if (badge != null)
                {
                    badgeEarned = new PortfolioBadgeDto
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

        var response = new TeacherMissionProgressResponse
        {
            MissionId = request.MissionId,
            NewProgress = (int)missionProgress.ProgressPercentage,
            Status = missionProgress.Status == ProgressStatus.Completed ? "completed" : "in-progress",
            BadgeEarned = badgeEarned
        };

        return RequestResult<TeacherMissionProgressResponse>.Success(response);
    }
}

