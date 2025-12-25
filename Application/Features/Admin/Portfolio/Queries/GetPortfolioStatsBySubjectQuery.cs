using API.Application.Features.Admin.Portfolio.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Portfolio.Queries;

public record GetPortfolioStatsBySubjectQuery(string? SubjectName = null) : IRequest<RequestResult<List<SubjectPortfolioStatsDto>>>;

public class GetPortfolioStatsBySubjectQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.General.Subjects> subjectRepository,
    IRepository<Domain.Entities.Portfolio.PortfolioFiles> portfolioRepository)
    : RequestHandlerBase<GetPortfolioStatsBySubjectQuery, RequestResult<List<SubjectPortfolioStatsDto>>>(parameters)
{
    public override async Task<RequestResult<List<SubjectPortfolioStatsDto>>> Handle(GetPortfolioStatsBySubjectQuery request, CancellationToken cancellationToken)
    {
        var subjectsQuery = subjectRepository.Get();
        
        if (!string.IsNullOrWhiteSpace(request.SubjectName))
        {
            subjectsQuery = subjectsQuery.Where(s => s.Name.ToLower().Contains(request.SubjectName.ToLower()));
        }

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var stats = await (
            from s in subjectsQuery
            join pf in portfolioRepository.Get() on s.ID equals pf.SubjectId into portfolioGroup
            from portfolio in portfolioGroup.DefaultIfEmpty()
            group portfolio by new { s.ID, s.Name } into g
            select new SubjectPortfolioStatsDto
            {
                SubjectName = g.Key.Name,
                TotalSubmissions = g.Count(p => p != null),
                ActiveStudents = g.Where(p => p != null).Select(p => p.StudentId).Distinct().Count(),
                RecentActivity = g.Count(p => p != null && 
                                        p.UploadedAt.Month == currentMonth && 
                                        p.UploadedAt.Year == currentYear)
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<SubjectPortfolioStatsDto>>.Success(stats);
    }
}
