using API.Application.Features.Admin.Evidence.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Evidence.Queries;

public record GetEvidenceStatsQuery : IRequest<RequestResult<EvidenceStatsDto>>;

public class GetEvidenceStatsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Portfolio.PortfolioFiles> portfolioRepository,
    IRepository<Domain.Entities.Teacher.CpdModules> cpdRepository,
    IRepository<Domain.Entities.Users.Badges> badgesRepository)
    : RequestHandlerBase<GetEvidenceStatsQuery, RequestResult<EvidenceStatsDto>>(parameters)
{
    public override async Task<RequestResult<EvidenceStatsDto>> Handle(GetEvidenceStatsQuery request, CancellationToken cancellationToken)
    {
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        // Count portfolios
        var totalPortfolios = await portfolioRepository.Get()
            .CountAsync(cancellationToken);

        var portfoliosThisMonth = await portfolioRepository.Get()
            .CountAsync(p => p.UploadedAt.Month == currentMonth && p.UploadedAt.Year == currentYear, cancellationToken);

        // Count CPD modules
        var totalCpd = await cpdRepository.Get()
            .CountAsync(c => c.IsActive, cancellationToken);

        var cpdThisMonth = await cpdRepository.Get()
            .CountAsync(c => c.IsActive && c.CreatedAt.Month == currentMonth && c.CreatedAt.Year == currentYear, cancellationToken);

        // Count badges
        var totalBadges = await badgesRepository.Get()
            .CountAsync(cancellationToken);

        var badgesThisMonth = await badgesRepository.Get()
            .CountAsync(b => b.CreatedAt.Month == currentMonth && b.CreatedAt.Year == currentYear, cancellationToken);

        // Calculate totals
        var totalEvidenceItems = totalPortfolios + totalCpd + totalBadges;
        var thisMonth = portfoliosThisMonth + cpdThisMonth + badgesThisMonth;

        // For pending review, we'll count recent portfolio uploads (last 7 days) as a simple heuristic
        // You can modify this logic based on your actual review system
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        var pendingReview = await portfolioRepository.Get()
            .CountAsync(p => p.UploadedAt >= sevenDaysAgo, cancellationToken);

        var result = new EvidenceStatsDto
        {
            TotalEvidenceItems = totalEvidenceItems,
            ThisMonth = thisMonth,
            ByType = new EvidenceByTypeDto
            {
                Portfolios = totalPortfolios,
                Cpd = totalCpd,
                Badges = totalBadges
            },
            PendingReview = pendingReview
        };

        return RequestResult<EvidenceStatsDto>.Success(result);
    }
}
