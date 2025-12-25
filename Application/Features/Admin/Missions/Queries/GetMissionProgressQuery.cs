using API.Application.Features.Admin.Missions.DTOs;
using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;
using MissionEntity = API.Domain.Entities.Missions.Missions;

namespace API.Application.Features.Admin.Missions.Queries;

public record GetMissionProgressQuery : IRequest<RequestResult<MissionProgressOverviewDto>>;

public class GetMissionProgressQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<StudentMissionProgress> missionProgressRepository,
    IRepository<MissionEntity> missionRepository,
    IRepository<ClassEntity> classRepository)
    : RequestHandlerBase<GetMissionProgressQuery, RequestResult<MissionProgressOverviewDto>>(parameters)
{
    public override async Task<RequestResult<MissionProgressOverviewDto>> Handle(
        GetMissionProgressQuery request,
        CancellationToken cancellationToken)
    {
        var totalStudents = await userRepository.Get(u => u.Role == ApplicationRole.Student)
            .CountAsync(cancellationToken);

        var studentsWithProgress = await missionProgressRepository.Get()
            .Select(mp => mp.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);

        var completedMissions = await missionProgressRepository.Get()
            .Where(mp => mp.Status == ProgressStatus.Completed)
            .CountAsync(cancellationToken);

        var allMissions = await missionRepository.Get().CountAsync(cancellationToken);
        var averageCompletionRate = totalStudents > 0 && allMissions > 0
            ? (decimal)completedMissions / (totalStudents * allMissions) * 100
            : 0;

        // Get progress by mission
        var missions = await missionRepository.Get().ToListAsync(cancellationToken);
        var missionProgressList = new List<MissionProgressDto>();

        foreach (var mission in missions)
        {
            var started = await missionProgressRepository.Get(mp => mp.MissionId == mission.ID)
                .Select(mp => mp.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            var completed = await missionProgressRepository.Get(mp => 
                mp.MissionId == mission.ID && 
                mp.Status == ProgressStatus.Completed)
                .CountAsync(cancellationToken);

            var completionRate = started > 0 ? (decimal)completed / started * 100 : 0;

            missionProgressList.Add(new MissionProgressDto
            {
                MissionId = mission.ID,
                MissionTitle = mission.Title,
                StudentsStarted = started,
                StudentsCompleted = completed,
                CompletionRate = completionRate
            });
        }

        // Get at-risk students (< 30% completion rate or no activity in 14 days)
        var students = await userRepository.Get(u => u.Role == ApplicationRole.Student)
            .ToListAsync(cancellationToken);

        var atRiskStudents = new List<AtRiskStudentDto>();

        foreach (var student in students)
        {
            var studentProgress = await missionProgressRepository.Get(mp => mp.StudentId == student.ID)
                .ToListAsync(cancellationToken);

            var missionsStarted = studentProgress.Count;
            var missionsCompleted = studentProgress.Count(mp => mp.Status == ProgressStatus.Completed);
            var completionRate = missionsStarted > 0 ? (decimal)missionsCompleted / missionsStarted * 100 : 0;
            var lastActivity = studentProgress.OrderByDescending(mp => mp.UpdatedAt).FirstOrDefault()?.UpdatedAt;

            var daysSinceActivity = lastActivity.HasValue 
                ? (DateTime.UtcNow - lastActivity.Value).Days 
                : 999;

            if (completionRate < 30 || daysSinceActivity > 14)
            {
                var classInfo = student.ClassID.HasValue
                    ? await classRepository.Get(c => c.ID == student.ClassID.Value).FirstOrDefaultAsync(cancellationToken)
                    : null;

                atRiskStudents.Add(new AtRiskStudentDto
                {
                    StudentId = student.ID,
                    StudentName = student.Name,
                    ClassName = classInfo?.Name ?? "No Class",
                    MissionsStarted = missionsStarted,
                    MissionsCompleted = missionsCompleted,
                    CompletionRate = completionRate,
                    LastActivityDate = lastActivity
                });
            }
        }

        var overview = new MissionProgressOverviewDto
        {
            TotalStudents = totalStudents,
            StudentsStarted = studentsWithProgress,
            StudentsCompleted = await missionProgressRepository.Get()
                .Where(mp => mp.Status == ProgressStatus.Completed)
                .Select(mp => mp.StudentId)
                .Distinct()
                .CountAsync(cancellationToken),
            AverageCompletionRate = averageCompletionRate,
            MissionProgress = missionProgressList,
            AtRiskStudents = atRiskStudents.OrderBy(s => s.CompletionRate).Take(20).ToList()
        };

        return RequestResult<MissionProgressOverviewDto>.Success(overview);
    }
}

