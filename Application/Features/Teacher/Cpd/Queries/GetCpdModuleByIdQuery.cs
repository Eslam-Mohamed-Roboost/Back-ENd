using System.Text.Json;
using API.Application.Features.Teacher.Cpd.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Cpd.Queries;

public record GetCpdModuleByIdQuery(long Id) : IRequest<RequestResult<CpdModuleDto>>;

public class GetCpdModuleByIdQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<CpdModules> modulesRepository,
    IRepository<TeacherCpdProgress> progressRepository)
    : RequestHandlerBase<GetCpdModuleByIdQuery, RequestResult<CpdModuleDto>>(parameters)
{
    public override async Task<RequestResult<CpdModuleDto>> Handle(GetCpdModuleByIdQuery request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var module = await modulesRepository.Get(x => x.ID == request.Id && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (module == null)
            return RequestResult<CpdModuleDto>.Failure(ErrorCode.NotFound, "Module not found");

        var progress = await progressRepository.Get(x => x.TeacherId == teacherId && x.ModuleId == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var evidenceFiles = new List<string>();
        if (!string.IsNullOrEmpty(progress?.EvidenceFiles))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<List<string>>(progress.EvidenceFiles);
                if (parsed != null) evidenceFiles = parsed;
            }
            catch
            {
            }
        }

        var dto = new CpdModuleDto
        {
            Id = module.ID,
            Title = module.Title,
            Description = module.Description ?? string.Empty,
            Duration = module.DurationMinutes,
            Status = progress?.Status switch
            {
                ProgressStatus.Completed => "completed",
                ProgressStatus.InProgress => "in-progress",
                _ => "not-started"
            },
            Icon = module.Icon ?? string.Empty,
            Color = module.Color ?? string.Empty,
            BgColor = module.BackgroundColor ?? string.Empty,
            VideoUrl = module.VideoUrl,
            VideoProvider = module.VideoProvider?.ToString().ToLowerInvariant(),
            GuideContent = module.GuideContent,
            FormUrl = module.FormUrl,
            EvidenceFiles = evidenceFiles,
            CompletedAt = progress?.CompletedAt,
            StartedAt = progress?.StartedAt,
            LastAccessedAt = progress?.LastAccessedAt
        };

        return RequestResult<CpdModuleDto>.Success(dto);
    }
}


