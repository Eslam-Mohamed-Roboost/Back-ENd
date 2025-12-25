using API.Application.Features.Admin.CPD.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.CPD.Queries;

public record GetCPDStatisticsQuery : IRequest<RequestResult<CPDStatisticsDto>>;

public class GetCPDStatisticsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository)
    : RequestHandlerBase<GetCPDStatisticsQuery, RequestResult<CPDStatisticsDto>>(parameters)
{
    public override async Task<RequestResult<CPDStatisticsDto>> Handle(GetCPDStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Get total teachers
        var totalTeachers = await userRepository.Get()
            .CountAsync(u => u.Role == ApplicationRole.Teacher, cancellationToken);

        // Get teachers with CPD progress
        var teacherWithProgress = await cpdProgressRepository.Get()
            .Where(cp => cp.Status == ProgressStatus.Completed)
            .Select(cp => cp.TeacherId)
            .Distinct()
            .CountAsync(cancellationToken);

        // Get total CPD hours
        var totalHours = await cpdProgressRepository.Get()
            .Where(cp => cp.Status == ProgressStatus.Completed)
            .SumAsync(cp => cp.HoursEarned ?? 0, cancellationToken);

        // Calculate average hours per teacher
        var averageHours = totalTeachers > 0 ? (int)(totalHours / totalTeachers) : 0;

        // Get top performer
        var topPerformer = await (
            from cp in cpdProgressRepository.Get()
            join u in userRepository.Get() on cp.TeacherId equals u.ID
            where cp.Status == ProgressStatus.Completed
            group cp by new { u.ID, u.Name } into g
            select new
            {
                g.Key.Name,
                TotalHours = g.Sum(x => x.HoursEarned ?? 0)
            })
            .OrderByDescending(x => x.TotalHours)
            .FirstOrDefaultAsync(cancellationToken);

        var result = new CPDStatisticsDto
        {
            TotalTeachers = totalTeachers,
            ActiveTeachers = teacherWithProgress,
            TotalCPDHours = (int)totalHours,
            AverageHoursPerTeacher = averageHours,
            TopPerformer = topPerformer?.Name ?? "N/A",
            TopPerformerHours = (int)(topPerformer?.TotalHours ?? 0)
        };

        return RequestResult<CPDStatisticsDto>.Success(result);
    }
}
