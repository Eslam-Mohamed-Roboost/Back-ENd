using API.Application.Features.Teacher.Missions.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Missions.Queries;

public record GetTeacherMissionsProgressQuery : IRequest<RequestResult<TeacherMissionsProgressSummaryDto>>;

public class GetTeacherMissionsProgressQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherMissions> missionsRepository,
    IRepository<TeacherMissionProgress> progressRepository)
    : RequestHandlerBase<GetTeacherMissionsProgressQuery, RequestResult<TeacherMissionsProgressSummaryDto>>(parameters)
{
    public override async Task<RequestResult<TeacherMissionsProgressSummaryDto>> Handle(GetTeacherMissionsProgressQuery request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get all enabled missions
        var allMissions = await missionsRepository.Get(x => x.IsEnabled).ToListAsync(cancellationToken);
        var totalMissions = allMissions.Count;

        // Get teacher's progress for all missions
        var progressList = await progressRepository.Get(x => x.TeacherId == teacherId)
            .ToListAsync(cancellationToken);

        var progressMap = progressList.ToDictionary(x => x.MissionId);

        var missionDtos = allMissions.Select(m =>
        {
            var progress = progressMap.GetValueOrDefault(m.ID);

            return new TeacherMissionProgressDto
            {
                MissionId = m.ID,
                MissionTitle = m.Title,
                Status = GetMissionStatus(progress),
                Progress = progress != null ? (int)progress.ProgressPercentage : 0,
                CompletedActivities = progress?.CompletedActivities ?? 0,
                TotalActivities = progress?.TotalActivities ?? 0,
                StartedAt = progress?.StartedAt,
                CompletedAt = progress?.CompletedAt
            };
        }).ToList();

        var completedMissions = missionDtos.Count(m => m.Status == "completed");
        var inProgressMissions = missionDtos.Count(m => m.Status == "in-progress");
        var notStartedMissions = missionDtos.Count(m => m.Status == "not-started");

        var summary = new TeacherMissionsProgressSummaryDto
        {
            TotalMissions = totalMissions,
            CompletedMissions = completedMissions,
            InProgressMissions = inProgressMissions,
            NotStartedMissions = notStartedMissions,
            Missions = missionDtos
        };

        return RequestResult<TeacherMissionsProgressSummaryDto>.Success(summary);
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

