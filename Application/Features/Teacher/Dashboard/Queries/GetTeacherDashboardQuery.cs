using API.Application.Features.Teacher.Cpd.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Entities.Gamification;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Dashboard.Queries;

public record GetTeacherDashboardQuery : IRequest<RequestResult<TeacherDashboardDto>>;

public class GetTeacherDashboardQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<CpdModules> modulesRepository,
    IRepository<TeacherCpdProgress> progressRepository,
    IRepository<TeacherBadgeSubmissions> badgeSubmissionsRepository,
    IRepository<StudentChallenges> studentChallengesRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetTeacherDashboardQuery, RequestResult<TeacherDashboardDto>>(parameters)
{
    public override async Task<RequestResult<TeacherDashboardDto>> Handle(GetTeacherDashboardQuery request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var modules = await modulesRepository.Get(x => x.IsActive).ToListAsync(cancellationToken);
        var progress = await progressRepository.Get(x => x.TeacherId == teacherId).ToListAsync(cancellationToken);

        var completedModules = progress.Count(x => x.Status == Domain.Enums.ProgressStatus.Completed);
        var totalModules = modules.Count;
        var hoursCompleted = progress.Where(x => x.HoursEarned.HasValue).Sum(x => x.HoursEarned!.Value);

        var lastActivity = progress
            .Select(x => x.LastAccessedAt ?? x.CompletedAt ?? x.StartedAt)
            .Where(d => d.HasValue)
            .Max();

        // Simple placeholders for target hours and streak
        var targetHours = 10m;
        var streak = 0;

        var cpdProgress = new CpdProgressDto
        {
            HoursCompleted = hoursCompleted,
            TargetHours = targetHours,
            CompletedModules = completedModules,
            TotalModules = totalModules,
            LastActivityDate = lastActivity,
            Streak = streak
        };

        // Badges earned (approved submissions with CPD hours)
        var badgesEarned = await badgeSubmissionsRepository.Get(x => x.TeacherId == teacherId && x.Status == Domain.Enums.SubmissionStatus.Approved)
            .CountAsync(cancellationToken);

        // Active students: distinct students who have challenge records (simple proxy)
        var activeStudents = await studentChallengesRepository.Get()
            .Select(x => x.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);

        var stats = new TeacherStatsDto
        {
            CpdHours = hoursCompleted,
            BadgesEarned = badgesEarned,
            ActiveStudents = activeStudents,
            CurrentStreak = streak
        };

        var dto = new TeacherDashboardDto
        {
            CpdProgress = cpdProgress,
            Stats = stats
        };

        return RequestResult<TeacherDashboardDto>.Success(dto);
    }
}


