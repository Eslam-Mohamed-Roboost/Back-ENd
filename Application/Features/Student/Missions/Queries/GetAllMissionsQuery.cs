using System.Text.Json;
using API.Domain.Entities.Missions;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MissionsEntity = API.Domain.Entities.Missions.Missions;

namespace API.Application.Features.Student.Missions.Queries;

public record MissionWithPrerequisitesDto(
    long Id,
    string Title,
    string Description,
    string Difficulty,
    int Points,
    decimal? HoursAwarded,
    DateTime? Deadline,
    bool IsLocked,
    List<long> PrerequisiteMissionIds,
    bool IsCompleted,
    int? Progress);

public record GetAllMissionsQuery : IRequest<RequestResult<List<MissionWithPrerequisitesDto>>>;

public class GetAllMissionsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionsEntity> missionsRepository,
    IRepository<StudentMissionProgress> progressRepository)
    : RequestHandlerBase<GetAllMissionsQuery, RequestResult<List<MissionWithPrerequisitesDto>>>(parameters)
{
    public override async Task<RequestResult<List<MissionWithPrerequisitesDto>>> Handle(
        GetAllMissionsQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get all enabled missions
        var missions = await missionsRepository
            .Get(m => m.IsEnabled)
            .OrderBy(m => m.Order)
            .ToListAsync(cancellationToken);

        // Get student's progress
        var progressRecords = await progressRepository
            .Get(p => p.StudentId == studentId)
            .ToListAsync(cancellationToken);

        var completedMissionIds = progressRecords
            .Where(p => p.Status == API.Domain.Enums.ProgressStatus.Completed)
            .Select(p => p.MissionId)
            .ToHashSet();

        var missionDtos = missions.Select(m =>
        {
            var progress = progressRecords.FirstOrDefault(p => p.MissionId == m.ID);
            var prerequisiteIds = ParsePrerequisiteIds(m.PrerequisiteMissionIds);
            var isLocked = !CanStartMission(prerequisiteIds, completedMissionIds);

            return new MissionWithPrerequisitesDto(
                m.ID,
                m.Title,
                m.Description ?? string.Empty,
                "Medium", // Default difficulty
                0, // Points not in current schema
                m.HoursAwarded,
                m.Deadline,
                isLocked,
                prerequisiteIds,
                progress?.Status == API.Domain.Enums.ProgressStatus.Completed,
                progress != null ? (int?)progress.ProgressPercentage : null);
        }).ToList();

        return RequestResult<List<MissionWithPrerequisitesDto>>.Success(missionDtos);
    }

    private List<long> ParsePrerequisiteIds(string? prerequisiteJson)
    {
        if (string.IsNullOrWhiteSpace(prerequisiteJson))
        {
            return new List<long>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<long>>(prerequisiteJson) ?? new List<long>();
        }
        catch
        {
            return new List<long>();
        }
    }

    private bool CanStartMission(List<long> prerequisiteIds, HashSet<long> completedMissionIds)
    {
        if (!prerequisiteIds.Any())
        {
            return true; // No prerequisites
        }

        // All prerequisites must be completed
        return prerequisiteIds.All(id => completedMissionIds.Contains(id));
    }
}
