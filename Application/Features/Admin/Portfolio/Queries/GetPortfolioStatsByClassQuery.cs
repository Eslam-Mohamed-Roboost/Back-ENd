using API.Application.Features.Admin.Portfolio.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Portfolio.Queries;

public record GetPortfolioStatsByClassQuery(string? ClassName = null) : IRequest<RequestResult<List<ClassPortfolioStatsDto>>>;

public class GetPortfolioStatsByClassQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.General.Classes> classRepository,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Portfolio.PortfolioFiles> portfolioRepository)
    : RequestHandlerBase<GetPortfolioStatsByClassQuery, RequestResult<List<ClassPortfolioStatsDto>>>(parameters)
{
    public override async Task<RequestResult<List<ClassPortfolioStatsDto>>> Handle(GetPortfolioStatsByClassQuery request, CancellationToken cancellationToken)
    {
        var classesQuery = classRepository.Get();
        
        if (!string.IsNullOrWhiteSpace(request.ClassName))
        {
            classesQuery = classesQuery.Where(c => c.Name.ToLower().Contains(request.ClassName.ToLower()));
        }

        var stats = await (
            from c in classesQuery
            join u in userRepository.Get() on c.ID equals u.ClassID into userGroup
            from user in userGroup.DefaultIfEmpty()
            where user == null || user.Role == ApplicationRole.Student
            group user by new { c.ID, c.Name, c.Grade } into g
            select new
            {
                g.Key.ID,
                g.Key.Name,
                g.Key.Grade,
                StudentIds = g.Where(u => u != null).Select(u => u.ID).ToList(),
                TotalStudents = g.Count(u => u != null)
            })
            .ToListAsync(cancellationToken);

        var classIds = stats.Select(s => s.ID).ToList();
        var studentIds = stats.SelectMany(s => s.StudentIds).ToList();

        var portfolioCounts = await portfolioRepository.Get()
            .Where(p => studentIds.Contains(p.StudentId))
            .GroupBy(p => p.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var result = stats.Select(s =>
        {
            var activeStudents = portfolioCounts.Count(pc => s.StudentIds.Contains(pc.StudentId));
            var completionRate = s.TotalStudents > 0 ? (int)((activeStudents * 100.0) / s.TotalStudents) : 0;

            return new ClassPortfolioStatsDto
            {
                ClassName = s.Name,
                Grade = s.Grade,
                TotalStudents = s.TotalStudents,
                ActivePortfolios = activeStudents,
                CompletionRate = completionRate
            };
        }).ToList();

        return RequestResult<List<ClassPortfolioStatsDto>>.Success(result);
    }
}
