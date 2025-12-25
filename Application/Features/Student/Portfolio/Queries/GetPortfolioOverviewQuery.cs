using API.Application.Features.Student.Badges.Queries;
using API.Application.Features.Student.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Application.Features.Student.Portfolio.Queries;

public record GetPortfolioOverviewQuery : IRequest<RequestResult<PortfolioOverviewResponse>>;

public class GetPortfolioOverviewQueryHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<GetPortfolioOverviewQuery, RequestResult<PortfolioOverviewResponse>>(parameters)
{
    public override async Task<RequestResult<PortfolioOverviewResponse>> Handle(GetPortfolioOverviewQuery request, CancellationToken cancellationToken)
    {
        // 1. Get Files
        var filesResult = await _mediator.Send(new GetStudentPortfolioFilesQuery(), cancellationToken);
        var allFiles = filesResult.Data ?? new List<PortfolioFileDto>();

        // 2. Get Feedback Count
        var feedbackResult = await _mediator.Send(new GetStudentFeedbackCountQuery(), cancellationToken);
        var totalFeedback = feedbackResult.Data;

        // 3. Get Badges Count
        var badgesResult = await _mediator.Send(new GetStudentBadgesCountQuery(), cancellationToken);
        var totalBadges = badgesResult.Data;

        // 4. Get Subjects
        var subjectsResult = await _mediator.Send(new GetAllActiveSubjectsQuery(), cancellationToken);
        var subjects = subjectsResult.Data ?? new List<SimpleSubjectDto>();

        // 5. Aggregate Results
        var totalFiles = allFiles.Count;

        var recentUploads = allFiles
            .OrderByDescending(x => x.UploadDate)
            .Take(3)
            .ToList();

        var filesBySubject = allFiles
            .GroupBy(f => f.SubjectId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var subjectPortfolios = subjects.Select(subject =>
        {
            filesBySubject.TryGetValue(subject.Id, out var filesForSubject);
            filesForSubject ??= new List<PortfolioFileDto>();

            var filesCount = filesForSubject.Count;
            var latestUpload = filesForSubject.Any() ? (DateTime?)filesForSubject.Max(x => x.UploadDate) : null;

            return new SubjectPortfolioDto
            {
                SubjectId = subject.Id,
                SubjectName = subject.Name,
                SubjectIcon = subject.Icon ?? "",
                Files = filesForSubject,
                Feedback = new(),
                Reflections = new(),
                Badges = new(),
                Stats = new PortfolioStatsDto
                {
                    FilesCount = filesCount,
                    LatestUploadDate = latestUpload,
                    FeedbackCount = 0,
                    BadgesCount = 0
                }
            };
        }).ToList();

        var response = new PortfolioOverviewResponse
        {
            TotalFiles = totalFiles,
            TotalFeedback = totalFeedback,
            TotalBadges = totalBadges,
            SubjectPortfolios = subjectPortfolios,
            RecentUploads = recentUploads
        };

        return RequestResult<PortfolioOverviewResponse>.Success(response);
    }
}
