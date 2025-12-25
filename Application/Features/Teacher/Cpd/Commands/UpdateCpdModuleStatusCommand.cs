using API.Application.Features.Teacher.Cpd.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Cpd.Commands;

public class UpdateCpdModuleStatusRequest
{
    public string Status { get; set; } = "not-started"; // not-started | in-progress | completed
}

public record UpdateCpdModuleStatusCommand(long ModuleId, UpdateCpdModuleStatusRequest Request) : IRequest<RequestResult<CpdModuleDto>>;

public class UpdateCpdModuleStatusCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<CpdModules> modulesRepository,
    IRepository<TeacherCpdProgress> progressRepository)
    : RequestHandlerBase<UpdateCpdModuleStatusCommand, RequestResult<CpdModuleDto>>(parameters)
{
    public override async Task<RequestResult<CpdModuleDto>> Handle(UpdateCpdModuleStatusCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var module = await modulesRepository.Get(x => x.ID == request.ModuleId && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (module == null)
            return RequestResult<CpdModuleDto>.Failure(ErrorCode.NotFound, "Module not found");

        var progress = await progressRepository.Get(x => x.TeacherId == teacherId && x.ModuleId == request.ModuleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (progress == null)
        {
            progress = new TeacherCpdProgress
            {
                TeacherId = teacherId,
                ModuleId = request.ModuleId
            };
            progressRepository.Add(progress);
        }

        var now = DateTime.UtcNow;
        progress.Status = request.Request.Status switch
        {
            "completed" => ProgressStatus.Completed,
            "in-progress" => ProgressStatus.InProgress,
            _ => ProgressStatus.NotStarted
        };

        if (progress.Status == ProgressStatus.InProgress && !progress.StartedAt.HasValue)
        {
            progress.StartedAt = now;
        }

        if (progress.Status == ProgressStatus.Completed)
        {
            progress.CompletedAt = now;
            progress.HoursEarned ??= module.DurationMinutes / 60m;
        }

        progress.LastAccessedAt = now;

        await progressRepository.SaveChangesAsync();

        var dto = new CpdModuleDto
        {
            Id = module.ID,
            Title = module.Title,
            Description = module.Description ?? string.Empty,
            Duration = module.DurationMinutes,
            Status = request.Request.Status,
            Icon = module.Icon ?? string.Empty,
            Color = module.Color ?? string.Empty,
            BgColor = module.BackgroundColor ?? string.Empty,
            VideoUrl = module.VideoUrl,
            VideoProvider = module.VideoProvider?.ToString().ToLowerInvariant(),
            GuideContent = module.GuideContent,
            FormUrl = module.FormUrl,
            CompletedAt = progress.CompletedAt,
            StartedAt = progress.StartedAt,
            LastAccessedAt = progress.LastAccessedAt
        };

        return RequestResult<CpdModuleDto>.Success(dto);
    }
}


