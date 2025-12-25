using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Application.Features.Admin.Evidence.DTOs;

namespace API.Application.Features.Admin.Evidence.Queries;

public record ExportEvidenceDataQuery(
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    List<string>? Subjects = null,
    List<string>? EvidenceTypes = null,
    string Format = "csv") 
    : IRequest<RequestResult<List<EvidenceExportItemDto>>>;

public class ExportEvidenceDataQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Portfolio.PortfolioFiles> portfolioRepository,
    IRepository<Domain.Entities.Teacher.TeacherBadgeSubmissions> badgeSubmissionRepository,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository,
    IRepository<Domain.Entities.General.Subjects> subjectsRepository)
    : RequestHandlerBase<ExportEvidenceDataQuery, RequestResult<List<EvidenceExportItemDto>>>(parameters)
{
    public override async Task<RequestResult<List<EvidenceExportItemDto>>> Handle(ExportEvidenceDataQuery request, CancellationToken cancellationToken)
    {
        var evidenceData = new List<EvidenceExportItemDto>();
        
        // Determine which evidence types to include
        var includePortfolios = request.EvidenceTypes == null || request.EvidenceTypes.Count == 0 || 
                                request.EvidenceTypes.Contains("Portfolios", StringComparer.OrdinalIgnoreCase);
        var includeBadges = request.EvidenceTypes == null || request.EvidenceTypes.Count == 0 || 
                           request.EvidenceTypes.Contains("Badges", StringComparer.OrdinalIgnoreCase);
        var includeCpd = request.EvidenceTypes == null || request.EvidenceTypes.Count == 0 || 
                        request.EvidenceTypes.Contains("Cpd", StringComparer.OrdinalIgnoreCase);

        // Portfolio Files Evidence
        if (includePortfolios)
        {
            var portfolioQuery = from pf in portfolioRepository.Get()
                                join u in userRepository.Get() on pf.StudentId equals u.ID
                                join s in subjectsRepository.Get() on pf.SubjectId equals s.ID
                                select new EvidenceExportItemDto
                                {
                                    EvidenceType = "Portfolio",
                                    UserName = u.Name,
                                    UserEmail = u.Email,
                                    Subject = s.Name,
                                    Title = pf.FileName,
                                    Description = pf.FileType.ToString(),
                                    Status = "Uploaded",
                                    CreatedAt = pf.UploadedAt,
                                    Link = pf.DownloadUrl
                                };

            if (request.DateFrom.HasValue)
            {
                portfolioQuery = portfolioQuery.Where(x => x.CreatedAt >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                portfolioQuery = portfolioQuery.Where(x => x.CreatedAt <= request.DateTo.Value);
            }

            if (request.Subjects != null && request.Subjects.Count > 0)
            {
                portfolioQuery = portfolioQuery.Where(x => request.Subjects.Contains(x.Subject));
            }

            var portfolioData = await portfolioQuery.ToListAsync(cancellationToken);
            evidenceData.AddRange(portfolioData);
        }

        // Badge Submissions Evidence
        if (includeBadges)
        {
            var badgeQuery = from bs in badgeSubmissionRepository.Get()
                            join u in userRepository.Get() on bs.TeacherId equals u.ID
                            select new EvidenceExportItemDto
                            {
                                EvidenceType = "Badge",
                                UserName = u.Name,
                                UserEmail = u.Email,
                                Subject = "N/A",
                                Title = $"Badge Submission #{bs.ID}",
                                Description = bs.SubmitterNotes ?? "",
                                Status = bs.Status.ToString(),
                                CreatedAt = bs.SubmittedAt,
                                Link = bs.EvidenceLink
                            };

            if (request.DateFrom.HasValue)
            {
                badgeQuery = badgeQuery.Where(x => x.CreatedAt >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                badgeQuery = badgeQuery.Where(x => x.CreatedAt <= request.DateTo.Value);
            }

            var badgeData = await badgeQuery.ToListAsync(cancellationToken);
            evidenceData.AddRange(badgeData);
        }

        // CPD Progress Evidence
        if (includeCpd)
        {
            var cpdQuery = from cp in cpdProgressRepository.Get()
                          join u in userRepository.Get() on cp.TeacherId equals u.ID
                          where cp.Status == ProgressStatus.Completed
                          select new EvidenceExportItemDto
                          {
                              EvidenceType = "CPD",
                              UserName = u.Name,
                              UserEmail = u.Email,
                              Subject = "Professional Development",
                              Title = $"CPD Module #{cp.ModuleId}",
                              Description = $"{cp.HoursEarned ?? 0} hours earned",
                              Status = cp.Status.ToString(),
                              CreatedAt = cp.CompletedAt ?? cp.StartedAt ?? DateTime.UtcNow,
                              Link = "N/A"
                          };

            if (request.DateFrom.HasValue)
            {
                cpdQuery = cpdQuery.Where(x => x.CreatedAt >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                cpdQuery = cpdQuery.Where(x => x.CreatedAt <= request.DateTo.Value);
            }

            var cpdData = await cpdQuery.ToListAsync(cancellationToken);
            evidenceData.AddRange(cpdData);
        }

        // Sort all evidence by date
        var sortedData = evidenceData.OrderByDescending(x => x.CreatedAt).ToList();
        
        return RequestResult<List<EvidenceExportItemDto>>.Success(sortedData);
    }
}
