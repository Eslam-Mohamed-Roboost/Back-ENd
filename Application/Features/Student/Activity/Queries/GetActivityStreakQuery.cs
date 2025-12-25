using API.Application.Features.Student.Activity.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Activity.Queries;

public record GetActivityStreakQuery : IRequest<RequestResult<ActivityStreakDto>>;

public class GetActivityStreakQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioFiles> portfolioFilesRepository)
    : RequestHandlerBase<GetActivityStreakQuery, RequestResult<ActivityStreakDto>>(parameters)
{
    public override async Task<RequestResult<ActivityStreakDto>> Handle(GetActivityStreakQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get recent activity (using portfolio uploads as proxy for activity)
        var recentUploads = await portfolioFilesRepository.Get(x => x.StudentId == studentId && x.UploadedAt >= DateTime.UtcNow.AddDays(-30))
            .Select(x => x.UploadedAt.Date)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Calculate current streak
        var currentStreak = CalculateCurrentStreak(recentUploads);
        var longestStreak = CalculateLongestStreak(recentUploads);

        var streak = new ActivityStreakDto
        {
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            LastActivityDate = recentUploads.Any() ? recentUploads.Max() : DateTime.MinValue,
            IsActiveToday = recentUploads.Any(d => d.Date == DateTime.UtcNow.Date),
            ActivityCalendar = recentUploads
        };

        return RequestResult<ActivityStreakDto>.Success(streak);
    }

    private static int CalculateCurrentStreak(List<DateTime> activityDates)
    {
        if (!activityDates.Any()) return 0;

        var sortedDates = activityDates.OrderByDescending(d => d).ToList();
        var streak = 0;
        var currentDate = DateTime.UtcNow.Date;

        foreach (var date in sortedDates)
        {
            if (date.Date == currentDate || date.Date == currentDate.AddDays(-1))
            {
                streak++;
                currentDate = date.Date.AddDays(-1);
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    private static int CalculateLongestStreak(List<DateTime> activityDates)
    {
        if (!activityDates.Any()) return 0;

        var sortedDates = activityDates.OrderBy(d => d).ToList();
        var maxStreak = 1;
        var currentStreak = 1;

        for (int i = 1; i < sortedDates.Count; i++)
        {
            if ((sortedDates[i].Date - sortedDates[i - 1].Date).Days == 1)
            {
                currentStreak++;
                maxStreak = Math.Max(maxStreak, currentStreak);
            }
            else
            {
                currentStreak = 1;
            }
        }

        return maxStreak;
    }
}
