using System.Text.RegularExpressions;
using API.Application.Features.Admin.Portfolio.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Portfolio.Queries;

public record GetPortfolioCompletionStatsQuery : IRequest<RequestResult<PortfolioCompletionStatsDto>>;

public class GetPortfolioCompletionStatsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Portfolio.PortfolioFiles> portfolioRepository,
    IRepository<Domain.Entities.General.Classes> classRepository,
    IRepository<Domain.Entities.General.Subjects> subjectRepository)
    : RequestHandlerBase<GetPortfolioCompletionStatsQuery, RequestResult<PortfolioCompletionStatsDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioCompletionStatsDto>> Handle(GetPortfolioCompletionStatsQuery request, CancellationToken cancellationToken)
    {
        // Get total students
        var students = await userRepository.Get()
            .CountAsync(u => u.Role == ApplicationRole.Student, cancellationToken);

        // Get students with active portfolios (at least one portfolio file)
        var activeStudents = await portfolioRepository.Get()
            .Select(p => p.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);

        // Calculate completion rate
        var completionRate = students > 0 ? (int)((activeStudents * 100.0) / students) : 0;

        // Get statistics by class using explicit joins
        var byClass = await (
            from c in classRepository.Get()
            join u in userRepository.Get() on c.ID equals u.ClassID.Value
            where u.Role == ApplicationRole.Student && u.ClassID.HasValue
            group u by new { c.ID, c.Name, c.Grade } into g
            select new
            {
                ClassId = g.Key.ID,
                ClassName = g.Key.Name,
                Grade = g.Key.Grade,
                StudentIds = g.Select(s => s.ID).ToList(),
                TotalStudents = g.Count()
            })
            .ToListAsync(cancellationToken);

        var byClassStats = new List<ClassPortfolioStatsDto>();
        foreach (var classData in byClass)
        {
            var classActivePortfolios = await portfolioRepository.Get()
                .Where(p => classData.StudentIds.Contains(p.StudentId))
                .Select(p => p.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            var classCompletionRate = classData.TotalStudents > 0 
                ? (int)((classActivePortfolios * 100.0) / classData.TotalStudents) 
                : 0;

            byClassStats.Add(new ClassPortfolioStatsDto
            {
                ClassName = classData.ClassName,
                Grade = classData.Grade,
                TotalStudents = classData.TotalStudents,
                ActivePortfolios = classActivePortfolios,
                CompletionRate = classCompletionRate
            });
        }

        // Get statistics by subject
        var subjectsList = await subjectRepository.Get()
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);

        var bySubject = new List<SubjectPortfolioStatsDto>();
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        foreach (var subject in subjectsList)
        {
            var totalSubmissions = await portfolioRepository.Get()
                .CountAsync(p => p.SubjectId == subject.ID, cancellationToken);

            var activeStudentsCount = await portfolioRepository.Get()
                .Where(p => p.SubjectId == subject.ID)
                .Select(p => p.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            var recentActivity = await portfolioRepository.Get()
                .CountAsync(p => p.SubjectId == subject.ID && p.UploadedAt >= thirtyDaysAgo, cancellationToken);

            bySubject.Add(new SubjectPortfolioStatsDto
            {
                SubjectName = subject.Name,
                TotalSubmissions = totalSubmissions,
                ActiveStudents = activeStudentsCount,
                RecentActivity = recentActivity
            });
        }

        // Get recent updates using explicit joins
        var recentUpdates = await (
            from p in portfolioRepository.Get()
            join u in userRepository.Get() on p.StudentId equals u.ID
            join c in classRepository.Get() on u.ClassID equals c.ID
            join s in subjectRepository.Get() on p.SubjectId equals s.ID
            orderby p.UploadedAt descending
            select new PortfolioUpdateDto
            {
                StudentId = p.StudentId,
                StudentName = u.Name,
                ClassName = c.Name,
                Subject = s.Name,
                UpdateType = "File Upload",
                Timestamp = p.UploadedAt,
                ItemCount = 1
            })
            .Take(10)
            .ToListAsync(cancellationToken);

        var result = new PortfolioCompletionStatsDto
        {
            TotalStudents = students,
            ActivePortfolios = activeStudents,
            CompletionRate = completionRate,
            ByClass = byClassStats,
            BySubject = bySubject,
            RecentUpdates = recentUpdates
        };

        return RequestResult<PortfolioCompletionStatsDto>.Success(result);
    }
}
