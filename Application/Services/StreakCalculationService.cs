using API.Domain.Entities.System;
using API.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Services;

public class StreakCalculationService : IStreakCalculationService
{
    private readonly IRepository<ActivityLogs> _activityLogsRepository;

    public StreakCalculationService(IRepository<ActivityLogs> activityLogsRepository)
    {
        _activityLogsRepository = activityLogsRepository;
    }

    public async Task<int> CalculateStudentStreakAsync(long studentId, CancellationToken cancellationToken = default)
    {
        // Get all activity dates for the student, grouped by date
        var activityDates = await _activityLogsRepository
            .Get(a => a.UserId == studentId)
            .Select(a => a.CreatedAt.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync(cancellationToken);

        if (!activityDates.Any())
        {
            return 0;
        }

        return CalculateConsecutiveDays(activityDates);
    }

    public async Task<int> CalculateTeacherStreakAsync(long teacherId, CancellationToken cancellationToken = default)
    {
        // Get all activity dates for the teacher, grouped by date
        var activityDates = await _activityLogsRepository
            .Get(a => a.UserId == teacherId)
            .Select(a => a.CreatedAt.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync(cancellationToken);

        if (!activityDates.Any())
        {
            return 0;
        }

        return CalculateConsecutiveDays(activityDates);
    }

    private int CalculateConsecutiveDays(List<DateTime> sortedDates)
    {
        if (!sortedDates.Any())
        {
            return 0;
        }

        var today = DateTime.UtcNow.Date;
        var mostRecentActivity = sortedDates[0];

        // If the most recent activity is not today or yesterday, streak is broken
        if (mostRecentActivity < today.AddDays(-1))
        {
            return 0;
        }

        int streak = 0;
        DateTime expectedDate = today;

        // If no activity today, start counting from yesterday
        if (mostRecentActivity < today)
        {
            expectedDate = today.AddDays(-1);
        }

        foreach (var activityDate in sortedDates)
        {
            // Check if this activity date matches our expected consecutive date
            if (activityDate == expectedDate)
            {
                streak++;
                expectedDate = expectedDate.AddDays(-1);
            }
            else if (activityDate < expectedDate)
            {
                // Gap found, streak is broken
                break;
            }
            // If activityDate > expectedDate, skip (multiple activities same day)
        }

        return streak;
    }
}

