using System.Text.Json;
using API.Application.Features.Teacher.Cpd.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Cpd.Commands;

public class UploadCpdEvidenceRequest
{
    public List<IFormFile> Files { get; set; } = new();
}

public record UploadCpdEvidenceCommand(long ModuleId, UploadCpdEvidenceRequest Request) : IRequest<RequestResult<CpdModuleDto>>;

public class UploadCpdEvidenceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<CpdModules> modulesRepository,
    IRepository<TeacherCpdProgress> progressRepository)
    : RequestHandlerBase<UploadCpdEvidenceCommand, RequestResult<CpdModuleDto>>(parameters)
{
    public override async Task<RequestResult<CpdModuleDto>> Handle(UploadCpdEvidenceCommand request, CancellationToken cancellationToken)
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

        var existing = new List<string>();
        if (!string.IsNullOrEmpty(progress.EvidenceFiles))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<List<string>>(progress.EvidenceFiles);
                if (parsed != null) existing = parsed;
            }
            catch
            {
            }
        }

        // NOTE: Real implementation should upload files to storage and store URLs.
        // For now, we'll just store file names as placeholders.
        foreach (var file in request.Request.Files)
        {
            existing.Add(file.FileName);
        }

        progress.EvidenceFiles = JsonSerializer.Serialize(existing);
        progress.LastAccessedAt = DateTime.UtcNow;

        await progressRepository.SaveChangesAsync();

        var dto = new CpdModuleDto
        {
            Id = module.ID,
            Title = module.Title,
            Description = module.Description ?? string.Empty,
            Duration = module.DurationMinutes,
            Status = progress.Status.ToString().ToLowerInvariant(),
            Icon = module.Icon ?? string.Empty,
            Color = module.Color ?? string.Empty,
            BgColor = module.BackgroundColor ?? string.Empty,
            VideoUrl = module.VideoUrl,
            VideoProvider = module.VideoProvider?.ToString().ToLowerInvariant(),
            GuideContent = module.GuideContent,
            FormUrl = module.FormUrl,
            EvidenceFiles = existing,
            CompletedAt = progress.CompletedAt,
            StartedAt = progress.StartedAt,
            LastAccessedAt = progress.LastAccessedAt
        };

        return RequestResult<CpdModuleDto>.Success(dto);
    }
}


