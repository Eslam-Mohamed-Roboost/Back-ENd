using API.Application.Features.Student.Missions.DTOs;
using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Missions.Queries;

public record GetAllMissionsQuery : IRequest<RequestResult<List<MissionDto>>>;

public class GetAllMissionsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Missions.Missions> missionsRepository,
    IRepository<StudentMissionProgress> progressRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<GetAllMissionsQuery, RequestResult<List<MissionDto>>>(parameters)
{
    public override async Task<RequestResult<List<MissionDto>>> Handle(GetAllMissionsQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get all enabled missions
        var missions = await missionsRepository.Get(x => x.IsEnabled).ToListAsync(cancellationToken);

        // Get student progress for all missions
        var progressMap = await progressRepository.Get(x => x.StudentId == studentId)
            .ToDictionaryAsync(x => x.MissionId, cancellationToken);

        // Get badges
        var badgeMap = await badgesRepository.Get()
            .ToDictionaryAsync(x => x.ID, cancellationToken);

        var result = missions.Select(m =>
        {
            var progress = progressMap.GetValueOrDefault(m.ID);
            var badge = badgeMap.GetValueOrDefault(m.BadgeId);

            return new MissionDto
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

        return RequestResult<List<MissionDto>>.Success(result);
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
