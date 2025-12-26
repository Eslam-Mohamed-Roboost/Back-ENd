using API.Application.Features.Teacher.Missions.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Missions.Queries;

public record GetTeacherMissionsQuery : IRequest<RequestResult<List<TeacherMissionDto>>>;

public class GetTeacherMissionsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherMissions> missionsRepository,
    IRepository<TeacherMissionProgress> progressRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<GetTeacherMissionsQuery, RequestResult<List<TeacherMissionDto>>>(parameters)
{
    public override async Task<RequestResult<List<TeacherMissionDto>>> Handle(GetTeacherMissionsQuery request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get all enabled missions
        var missions = await missionsRepository.Get(x => x.IsEnabled).ToListAsync(cancellationToken);

        // Get teacher progress for all missions
        var progressMap = await progressRepository.Get(x => x.TeacherId == teacherId)
            .ToDictionaryAsync(x => x.MissionId, cancellationToken);

        // Get badges
        var badgeMap = await badgesRepository.Get()
            .ToDictionaryAsync(x => x.ID, cancellationToken);

        var result = missions.Select(m =>
        {
            var progress = progressMap.GetValueOrDefault(m.ID);
            var badge = badgeMap.GetValueOrDefault(m.BadgeId);

            return new TeacherMissionDto
            {
                Id = m.ID,
                Title = m.Title,
                Description = m.Description ?? "",
                Icon = m.Icon ?? "📚",
                Status = GetMissionStatus(progress),
                Progress = progress != null ? (int)progress.ProgressPercentage : 0,
                Badge = badge?.Name ?? "",
                Duration = $"{m.EstimatedMinutes} mins",
                Requirements = new() // TODO: Implement prerequisites logic
            };
        }).OrderBy(x => x.Id).ToList();

        return RequestResult<List<TeacherMissionDto>>.Success(result);
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

