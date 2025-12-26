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

        // Get completed mission IDs for prerequisite checking
        var completedMissionIds = progressMap
            .Where(p => p.Value.Status == ProgressStatus.Completed)
            .Select(p => p.Key)
            .ToHashSet();

        var result = missions.Select(m =>
        {
            var progress = progressMap.GetValueOrDefault(m.ID);
            var badge = badgeMap.GetValueOrDefault(m.BadgeId);

            // Determine requirements/prerequisites
            var requirements = new List<string>();
            
            // If mission has Order > 1, check for previous missions that need to be completed
            if (m.Order > 1)
            {
                // Find missions with lower Order that are not completed
                var previousMissions = missions
                    .Where(prev => prev.Order < m.Order && prev.IsEnabled)
                    .OrderBy(prev => prev.Order)
                    .ToList();

                foreach (var prevMission in previousMissions)
                {
                    if (!completedMissionIds.Contains(prevMission.ID))
                    {
                        requirements.Add($"Complete: {prevMission.Title}");
                    }
                }

                // If all previous missions are completed, mission is unlocked
                if (requirements.Count == 0 && progress == null)
                {
                    // Mission is unlocked but not started yet
                    requirements.Add("Ready to start");
                }
            }
            else
            {
                // First mission(s) with Order = 1 have no prerequisites
                requirements.Add("No prerequisites");
            }

            // Determine status based on prerequisites and progress
            var missionStatus = GetMissionStatus(progress, requirements, m.Order, completedMissionIds, missions);

            return new MissionDto
            {
                Id = m.ID,
                Title = m.Title,
                Description = m.Description ?? "",
                Icon = m.Icon ?? "📚",
                Status = missionStatus,
                Progress = progress != null ? (int)progress.ProgressPercentage : 0,
                Badge = badge?.Name ?? "",
                Duration = $"{m.EstimatedMinutes} mins",
                Requirements = requirements
            };
        }).OrderBy(x => x.Id).ToList();

        return RequestResult<List<MissionDto>>.Success(result);
    }

    private static string GetMissionStatus(
        StudentMissionProgress? progress,
        List<string> requirements,
        int missionOrder,
        HashSet<long> completedMissionIds,
        List<Domain.Entities.Missions.Missions> allMissions)
    {
        // If mission has progress, use progress status
        if (progress != null)
        {
            return progress.Status switch
            {
                ProgressStatus.Completed => "completed",
                ProgressStatus.InProgress => "in-progress",
                ProgressStatus.NotStarted => "not-started",
                _ => "locked"
            };
        }

        // No progress - check if mission is locked due to prerequisites
        if (missionOrder > 1)
        {
            // Check if all previous missions are completed
            var previousMissions = allMissions
                .Where(m => m.Order < missionOrder && m.IsEnabled)
                .ToList();

            if (previousMissions.Any() && previousMissions.Any(prev => !completedMissionIds.Contains(prev.ID)))
            {
                // Some previous missions are not completed - mission is locked
                return "locked";
            }
        }

        // Mission is unlocked but not started
        return "not-started";
    }
}
