using API.Application.Features.Teacher.Cpd.DTOs;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Cpd.Queries;

public record GetCpdProgressQuery : IRequest<RequestResult<CpdProgressDto>>;

public class GetCpdProgressQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<CpdModules> modulesRepository,
    IRepository<TeacherCpdProgress> progressRepository)
    : RequestHandlerBase<GetCpdProgressQuery, RequestResult<CpdProgressDto>>(parameters)
{
    public override async Task<RequestResult<CpdProgressDto>> Handle(GetCpdProgressQuery request, CancellationToken cancellationToken)
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

        // TODO: Implement real CPD target hours and streak logic; using simple defaults for now
        var targetHours = 10m;
        var streak = 0;

        var dto = new CpdProgressDto
        {
            HoursCompleted = hoursCompleted,
            TargetHours = targetHours,
            CompletedModules = completedModules,
            TotalModules = totalModules,
            LastActivityDate = lastActivity
        };

        dto.Streak = streak;

        return RequestResult<CpdProgressDto>.Success(dto);
    }
}


