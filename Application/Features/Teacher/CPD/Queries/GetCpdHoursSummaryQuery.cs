using API.Application.Features.Teacher.CPD.DTOs;
using API.Application.Services;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.CPD.Queries;

public record GetCpdHoursSummaryQuery : IRequest<RequestResult<CpdHoursSummaryDto>>;

public class GetCpdHoursSummaryQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherCpdProgress> cpdProgressRepository,
    IHoursTrackingService hoursTrackingService)
    : RequestHandlerBase<GetCpdHoursSummaryQuery, RequestResult<CpdHoursSummaryDto>>(parameters)
{
    public override async Task<RequestResult<CpdHoursSummaryDto>> Handle(GetCpdHoursSummaryQuery request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get total CPD hours
        var totalHours = await hoursTrackingService.GetTotalCpdHoursAsync(teacherId, cancellationToken);

        // Calculate current year hours
        var yearStart = new DateTime(DateTime.UtcNow.Year, 1, 1);
        var thisYearHours = await cpdProgressRepository
            .Get(x => x.TeacherId == teacherId && 
                     x.CompletedAt.HasValue && 
                     x.CompletedAt.Value >= yearStart &&
                     x.HoursEarned.HasValue)
            .SumAsync(x => x.HoursEarned ?? 0, cancellationToken);

        // Get recent CPD activities
        var recentActivities = await cpdProgressRepository
            .Get(x => x.TeacherId == teacherId && 
                     x.CompletedAt.HasValue &&
                     x.HoursEarned.HasValue)
            .OrderByDescending(x => x.CompletedAt)
            .Take(10)
            .Select(x => new CpdHoursEntryDto
            {
                Id = x.ID,
                ModuleId = x.ModuleId,
                HoursEarned = x.HoursEarned ?? 0,
                CompletedDate = x.CompletedAt!.Value
            })
            .ToListAsync(cancellationToken);

        // Annual goal (configurable, default 20 hours)
        const decimal annualGoal = 20m;
        var progressPercentage = annualGoal > 0 ? (thisYearHours / annualGoal * 100) : 0;

        var summary = new CpdHoursSummaryDto
        {
            TotalHours = totalHours,
            ThisYearHours = thisYearHours,
            AnnualGoal = annualGoal,
            ProgressPercentage = Math.Min(progressPercentage, 100),
            RecentActivities = recentActivities
        };

        return RequestResult<CpdHoursSummaryDto>.Success(summary);
    }
}

