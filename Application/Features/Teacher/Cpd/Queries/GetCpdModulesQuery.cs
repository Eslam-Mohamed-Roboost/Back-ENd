using System.Text.Json;
using API.Application.Features.Teacher.Cpd.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Cpd.Queries;

public record GetCpdModulesQuery : IRequest<RequestResult<List<CpdModuleDto>>>;

public class GetCpdModulesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<CpdModules> modulesRepository,
    IRepository<TeacherCpdProgress> progressRepository)
    : RequestHandlerBase<GetCpdModulesQuery, RequestResult<List<CpdModuleDto>>>(parameters)
{
    public override async Task<RequestResult<List<CpdModuleDto>>> Handle(GetCpdModulesQuery request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var modules = await modulesRepository.Get(x => x.IsActive)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        var progressMap = await progressRepository.Get(x => x.TeacherId == teacherId)
            .ToDictionaryAsync(x => x.ModuleId, cancellationToken);

        var result = modules.Select(m =>
        {
            progressMap.TryGetValue(m.ID, out var p);

            var evidenceFiles = new List<string>();
            if (!string.IsNullOrEmpty(p?.EvidenceFiles))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<List<string>>(p.EvidenceFiles);
                    if (parsed != null) evidenceFiles = parsed;
                }
                catch
                {
                    // ignore malformed JSON
                }
            }

            return new CpdModuleDto
            {
                Id = m.ID,
                Title = m.Title,
                Description = m.Description ?? string.Empty,
                Duration = m.DurationMinutes,
                Status = p?.Status switch
                {
                    ProgressStatus.Completed => "completed",
                    ProgressStatus.InProgress => "in-progress",
                    _ => "not-started"
                },
                Icon = m.Icon ?? string.Empty,
                Color = m.Color ?? string.Empty,
                BgColor = m.BackgroundColor ?? string.Empty,
                VideoUrl = m.VideoUrl,
                VideoProvider = m.VideoProvider?.ToString().ToLowerInvariant(),
                GuideContent = m.GuideContent,
                FormUrl = m.FormUrl,
                EvidenceFiles = evidenceFiles,
                CompletedAt = p?.CompletedAt,
                StartedAt = p?.StartedAt,
                LastAccessedAt = p?.LastAccessedAt
            };
        }).ToList();

        return RequestResult<List<CpdModuleDto>>.Success(result);
    }
}


