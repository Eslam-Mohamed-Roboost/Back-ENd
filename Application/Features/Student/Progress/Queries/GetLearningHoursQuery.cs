using API.Application.Features.Student.Progress.DTOs;
using API.Application.Services;
using API.Domain.Entities.General;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Progress.Queries;

public record GetLearningHoursQuery(DateTime? StartDate = null, DateTime? EndDate = null) : IRequest<RequestResult<LearningHoursSummaryDto>>;

public class GetLearningHoursQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<LearningHours> learningHoursRepository,
    IHoursTrackingService hoursTrackingService)
    : RequestHandlerBase<GetLearningHoursQuery, RequestResult<LearningHoursSummaryDto>>(parameters)
{
    public override async Task<RequestResult<LearningHoursSummaryDto>> Handle(GetLearningHoursQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get total hours
        var totalHours = await hoursTrackingService.GetTotalLearningHoursAsync(studentId, cancellationToken);

        // Calculate date ranges
        var now = DateTime.UtcNow;
        var weekStart = now.AddDays(-(int)now.DayOfWeek);
        var monthStart = new DateTime(now.Year, now.Month, 1);

        // Get this week's hours
        var thisWeekHours = await learningHoursRepository
            .Get(x => x.StudentId == studentId && x.ActivityDate >= weekStart)
            .SumAsync(x => x.HoursEarned, cancellationToken);

        // Get this month's hours
        var thisMonthHours = await learningHoursRepository
            .Get(x => x.StudentId == studentId && x.ActivityDate >= monthStart)
            .SumAsync(x => x.HoursEarned, cancellationToken);

        // Get breakdown by activity type
        var byActivityType = await learningHoursRepository
            .Get(x => x.StudentId == studentId)
            .GroupBy(x => x.ActivityType)
            .Select(g => new ActivityHoursBreakdownDto
            {
                ActivityType = g.Key.ToString(),
                TotalHours = g.Sum(x => x.HoursEarned),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        // Get recent activities
        var recentActivities = await learningHoursRepository
            .Get(x => x.StudentId == studentId)
            .OrderByDescending(x => x.ActivityDate)
            .Take(10)
            .Select(x => new LearningHoursEntryDto
            {
                Id = x.ID,
                ActivityType = x.ActivityType.ToString(),
                ActivityId = x.ActivityId,
                HoursEarned = x.HoursEarned,
                EarnedDate = x.ActivityDate
            })
            .ToListAsync(cancellationToken);

        var summary = new LearningHoursSummaryDto
        {
            TotalHours = totalHours,
            ThisWeekHours = thisWeekHours,
            ThisMonthHours = thisMonthHours,
            ByActivityType = byActivityType,
            RecentActivities = recentActivities
        };

        return RequestResult<LearningHoursSummaryDto>.Success(summary);
    }
}

