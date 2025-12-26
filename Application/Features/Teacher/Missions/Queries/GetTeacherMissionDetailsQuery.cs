using API.Application.Features.Teacher.Missions.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Missions.Queries;

public record GetTeacherMissionDetailsQuery(long MissionId) : IRequest<RequestResult<TeacherMissionDetailDto>>;

public class GetTeacherMissionDetailsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherMissions> missionsRepository,
    IRepository<TeacherMissionProgress> progressRepository,
    IRepository<TeacherActivities> activitiesRepository,
    IRepository<TeacherActivityProgress> activityProgressRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<GetTeacherMissionDetailsQuery, RequestResult<TeacherMissionDetailDto>>(parameters)
{
    public override async Task<RequestResult<TeacherMissionDetailDto>> Handle(GetTeacherMissionDetailsQuery request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var mission = await missionsRepository.Get(x => x.ID == request.MissionId).FirstOrDefaultAsync(cancellationToken);
        if (mission == null)
            return RequestResult<TeacherMissionDetailDto>.Failure(ErrorCode.NotFound, "Mission not found");

        var progress = await progressRepository.Get(x => x.TeacherId == teacherId && x.MissionId == request.MissionId)
            .FirstOrDefaultAsync(cancellationToken);

        var badge = await badgesRepository.Get(x => x.ID == mission.BadgeId).FirstOrDefaultAsync(cancellationToken);

        // Get activities for this teacher mission
        var activities = await activitiesRepository.Get(x => x.MissionId == request.MissionId)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        var activityProgressMap = await activityProgressRepository.Get(x => x.TeacherId == teacherId && x.MissionId == request.MissionId)
            .ToDictionaryAsync(x => x.ActivityId, cancellationToken);

        var activityDtos = activities.Select(a => new TeacherMissionActivityDto
        {
            Id = a.ID,
            Type = a.Type.ToString(),
            Title = a.Title,
            Content = "",
            Completed = activityProgressMap.ContainsKey(a.ID) && activityProgressMap[a.ID].IsCompleted,
            Order = a.Order
        }).ToList();

        var result = new TeacherMissionDetailDto
        {
            Id = mission.ID,
            Title = mission.Title,
            Description = mission.Description ?? "",
            Icon = mission.Icon ?? "📚",
            Status = GetMissionStatus(progress),
            Progress = progress != null ? (int)progress.ProgressPercentage : 0,
            Badge = badge?.Name ?? "",
            Activities = activityDtos,
            Resources = new() // TODO: Implement learning resources if entity exists
        };

        return RequestResult<TeacherMissionDetailDto>.Success(result);
    }

    private static string GetMissionStatus(TeacherMissionProgress? progress)
    {
        if (progress == null) return "not-started";
        
        return progress.Status switch
        {
            ProgressStatus.Completed => "completed",
            ProgressStatus.InProgress => "in-progress",
            ProgressStatus.NotStarted => "not-started",
            _ => "locked"
        };
    }
}

