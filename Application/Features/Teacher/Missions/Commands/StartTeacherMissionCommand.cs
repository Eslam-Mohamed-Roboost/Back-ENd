using API.Application.Features.Teacher.Missions.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Missions.Commands;

public record StartTeacherMissionCommand(long MissionId) : IRequest<RequestResult<TeacherMissionProgressDto>>;

public class StartTeacherMissionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherMissions> missionsRepository,
    IRepository<TeacherMissionProgress> progressRepository,
    IRepository<TeacherActivities> activitiesRepository)
    : RequestHandlerBase<StartTeacherMissionCommand, RequestResult<TeacherMissionProgressDto>>(parameters)
{
    public override async Task<RequestResult<TeacherMissionProgressDto>> Handle(StartTeacherMissionCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var mission = await missionsRepository.Get(x => x.ID == request.MissionId && x.IsEnabled)
            .FirstOrDefaultAsync(cancellationToken);

        if (mission == null)
            return RequestResult<TeacherMissionProgressDto>.Failure(ErrorCode.NotFound, "Mission not found");

        // Check if already started
        var existingProgress = await progressRepository.Get(x => x.TeacherId == teacherId && x.MissionId == request.MissionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingProgress != null)
        {
            return RequestResult<TeacherMissionProgressDto>.Failure(ErrorCode.BadRequest, "Mission already started");
        }

        // Get total activities
        var totalActivities = await activitiesRepository.Get(x => x.MissionId == request.MissionId).CountAsync(cancellationToken);

        var progress = new TeacherMissionProgress
        {
            TeacherId = teacherId,
            MissionId = request.MissionId,
            Status = ProgressStatus.InProgress,
            StartedAt = DateTime.UtcNow,
            TotalActivities = totalActivities,
            CompletedActivities = 0,
            ProgressPercentage = 0
        };

        progressRepository.Add(progress);
        await progressRepository.SaveChangesAsync(cancellationToken);

        var dto = new TeacherMissionProgressDto
        {
            MissionId = progress.MissionId,
            MissionTitle = mission.Title,
            Status = "in-progress",
            Progress = 0,
            CompletedActivities = 0,
            TotalActivities = totalActivities,
            StartedAt = progress.StartedAt,
            CompletedAt = null
        };

        return RequestResult<TeacherMissionProgressDto>.Success(dto);
    }
}

