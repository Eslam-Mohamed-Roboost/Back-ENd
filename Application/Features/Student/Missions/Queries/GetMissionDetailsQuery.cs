using API.Application.Features.Student.Missions.DTOs;
using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Missions.Queries;

public record GetMissionDetailsQuery(long MissionId) : IRequest<RequestResult<MissionDetailDto>>;

public class GetMissionDetailsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Missions.Missions> missionsRepository,
    IRepository<StudentMissionProgress> progressRepository,
    IRepository<Activities> activitiesRepository,
    IRepository<StudentActivityProgress> activityProgressRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<GetMissionDetailsQuery, RequestResult<MissionDetailDto>>(parameters)
{
    public override async Task<RequestResult<MissionDetailDto>> Handle(GetMissionDetailsQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var mission = await missionsRepository.Get(x => x.ID == request.MissionId).FirstOrDefaultAsync(cancellationToken);
        if (mission == null)
            return RequestResult<MissionDetailDto>.Failure(ErrorCode.NotFound);

        var progress = await progressRepository.Get(x => x.StudentId == studentId && x.MissionId == request.MissionId)
            .FirstOrDefaultAsync(cancellationToken);

        var badge = await badgesRepository.Get(x => x.ID == mission.BadgeId).FirstOrDefaultAsync(cancellationToken);

        // Get activities
        var activities = await activitiesRepository.Get(x => x.MissionId == request.MissionId)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        var activityProgressMap = await activityProgressRepository.Get(x => x.StudentId == studentId && x.MissionId == request.MissionId)
            .ToDictionaryAsync(x => x.ActivityId, cancellationToken);

        var activityDtos = activities.Select(a => new MissionActivityDto
        {
            Id = a.ID,
            Type = a.Type.ToString(),
            Title = a.Title,
            Content = "",
            Completed = activityProgressMap.ContainsKey(a.ID) && activityProgressMap[a.ID].CompletedAt.HasValue,
            Order = a.Order
        }).ToList();

        var result = new MissionDetailDto
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

        return RequestResult<MissionDetailDto>.Success(result);
    }

    private static string GetMissionStatus(StudentMissionProgress? progress)
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
