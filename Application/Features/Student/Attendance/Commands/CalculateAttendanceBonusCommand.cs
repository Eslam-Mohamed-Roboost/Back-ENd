using API.Domain.Entities.Academic;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Attendance.Commands;

public record CalculateAttendanceBonusCommand : IRequest<RequestResult<AttendanceBonusResult>>;

public class AttendanceBonusResult
{
    public int PointsEarned { get; set; }
    public List<long> BadgeIds { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class CalculateAttendanceBonusCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentAttendance> attendanceRepository)
    : RequestHandlerBase<CalculateAttendanceBonusCommand, RequestResult<AttendanceBonusResult>>(parameters)
{
    public override async Task<RequestResult<AttendanceBonusResult>> Handle(CalculateAttendanceBonusCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var now = DateTime.UtcNow;

        // Get attendance records for the last 30 days
        var thirtyDaysAgo = now.AddDays(-30);
        var recentAttendance = await attendanceRepository.Get(x =>
            x.StudentId == studentId &&
            x.AttendanceDate >= thirtyDaysAgo)
            .OrderBy(x => x.AttendanceDate)
            .ToListAsync(cancellationToken);

        if (!recentAttendance.Any())
        {
            return RequestResult<AttendanceBonusResult>.Success(new AttendanceBonusResult
            {
                PointsEarned = 0,
                BadgeIds = new List<long>(),
                Message = "No attendance records found"
            });
        }

        var pointsEarned = 0;
        var badgeIds = new List<long>();
        var messages = new List<string>();

        // Check for perfect week (7 consecutive days)
        var weekStart = now.AddDays(-(int)now.DayOfWeek);
        var weekAttendance = recentAttendance.Where(a => a.AttendanceDate >= weekStart).ToList();
        var weekPresentCount = weekAttendance.Count(a => 
            a.Status == AttendanceStatus.Present || 
            a.Status == AttendanceStatus.Late || 
            a.Status == AttendanceStatus.Excused);

        if (weekPresentCount >= 5) // 5 school days in a week
        {
            pointsEarned += 50; // Bonus points for perfect week
            messages.Add("Perfect attendance this week! +50 points");
        }

        // Check for perfect month (all days present)
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthAttendance = recentAttendance.Where(a => a.AttendanceDate >= monthStart).ToList();
        var monthPresentCount = monthAttendance.Count(a => 
            a.Status == AttendanceStatus.Present || 
            a.Status == AttendanceStatus.Late || 
            a.Status == AttendanceStatus.Excused);
        var monthTotalDays = monthAttendance.Count;

        if (monthTotalDays > 0 && monthPresentCount == monthTotalDays && monthTotalDays >= 15)
        {
            pointsEarned += 200; // Bonus points for perfect month
            badgeIds.Add(1); // TODO: Replace with actual "Perfect Attendance" badge ID
            messages.Add("Perfect attendance this month! +200 points");
        }

        // Check for attendance streak (consecutive present days)
        var streak = CalculateAttendanceStreak(recentAttendance);
        if (streak >= 10)
        {
            pointsEarned += streak * 5; // 5 points per day in streak
            messages.Add($"Attendance streak of {streak} days! +{streak * 5} points");
        }

        var result = new AttendanceBonusResult
        {
            PointsEarned = pointsEarned,
            BadgeIds = badgeIds,
            Message = string.Join(" ", messages)
        };

        return RequestResult<AttendanceBonusResult>.Success(result);
    }

    private static int CalculateAttendanceStreak(List<StudentAttendance> records)
    {
        if (!records.Any()) return 0;

        var sortedRecords = records.OrderByDescending(x => x.AttendanceDate).ToList();
        var streak = 0;
        var currentDate = DateTime.UtcNow.Date;

        foreach (var record in sortedRecords)
        {
            var recordDate = record.AttendanceDate.Date;
            
            if (recordDate == currentDate || recordDate == currentDate.AddDays(-streak))
            {
                if (record.Status == AttendanceStatus.Present || 
                    record.Status == AttendanceStatus.Late || 
                    record.Status == AttendanceStatus.Excused)
                {
                    if (streak == 0)
                        currentDate = recordDate;
                    streak++;
                    currentDate = currentDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return streak;
    }
}

