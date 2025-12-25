using API.Application.Features.Admin.CPD.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.CPD.Queries;

public record GetCPDByMonthQuery(int MonthsBack = 6) : IRequest<RequestResult<List<CPDByMonthDto>>>;

public class GetCPDByMonthQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository)
    : RequestHandlerBase<GetCPDByMonthQuery, RequestResult<List<CPDByMonthDto>>>(parameters)
{
    public override async Task<RequestResult<List<CPDByMonthDto>>> Handle(GetCPDByMonthQuery request, CancellationToken cancellationToken)
    {
        var startDate = DateTime.UtcNow.AddMonths(-request.MonthsBack);

        var monthlyData = await cpdProgressRepository.Get()
            .Where(cp => cp.Status == ProgressStatus.Completed && cp.CompletedAt >= startDate)
            .GroupBy(cp => new
            {
                Year = cp.CompletedAt!.Value.Year,
                Month = cp.CompletedAt.Value.Month
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Hours = g.Sum(x => x.HoursEarned ?? 0)
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync(cancellationToken);

        var result = monthlyData.Select(m => new CPDByMonthDto
        {
            Month = new DateTime(m.Year, m.Month, 1).ToString("MMM yyyy"),
            Hours = (int)m.Hours
        }).ToList();

        return RequestResult<List<CPDByMonthDto>>.Success(result);
    }
}
