using API.Application.Features.Student.Attendance.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.General;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Attendance.Queries;

public record GetAttendanceStatisticsQuery : IRequest<RequestResult<AttendanceStatisticsDto>>;

public class GetAttendanceStatisticsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentAttendance> attendanceRepository,
    IRepository<Classes> classRepository)
    : RequestHandlerBase<GetAttendanceStatisticsQuery, RequestResult<AttendanceStatisticsDto>>(parameters)
{
    public override async Task<RequestResult<AttendanceStatisticsDto>> Handle(GetAttendanceStatisticsQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get all attendance records for this student
        var allRecords = await attendanceRepository.Get(x => x.StudentId == studentId)
            .OrderBy(x => x.AttendanceDate)
            .ToListAsync(cancellationToken);

        if (!allRecords.Any())
        {
            return RequestResult<AttendanceStatisticsDto>.Success(new AttendanceStatisticsDto
            {
                TotalDays = 0,
                PresentDays = 0,
                AbsentDays = 0,
                LateDays = 0,
                ExcusedDays = 0,
                AttendancePercentage = 0,
                CurrentStreak = 0,
                LongestStreak = 0,
                RecentHistory = new List<StudentAttendanceHistoryDto>()
            });
        }

        var totalDays = allRecords.Count;
        var presentDays = allRecords.Count(x => x.Status == AttendanceStatus.Present);
        var absentDays = allRecords.Count(x => x.Status == AttendanceStatus.Absent);
        var lateDays = allRecords.Count(x => x.Status == AttendanceStatus.Late);
        var excusedDays = allRecords.Count(x => x.Status == AttendanceStatus.Excused);

        // Calculate attendance percentage (Present + Late + Excused count as attended)
        var attendedDays = presentDays + lateDays + excusedDays;
        var attendancePercentage = totalDays > 0 ? (decimal)attendedDays / totalDays * 100 : 0;

        // Calculate streaks
        var currentStreak = CalculateCurrentStreak(allRecords);
        var longestStreak = CalculateLongestStreak(allRecords);

        // Get recent history (last 20 records)
        var recentRecords = allRecords
            .OrderByDescending(x => x.AttendanceDate)
            .Take(20)
            .ToList();

        // Get class names
        var classIds = recentRecords.Select(a => a.ClassId).Distinct().ToList();
        var classes = await classRepository.Get(x => classIds.Contains(x.ID))
            .ToDictionaryAsync(x => x.ID, cancellationToken);

        var recentHistory = recentRecords.Select(a => new StudentAttendanceHistoryDto
        {
            Date = a.AttendanceDate,
            ClassId = a.ClassId,
            ClassName = classes.GetValueOrDefault(a.ClassId)?.Name ?? "Unknown",
            Status = a.Status.ToString(),
            IsAutomatic = a.IsAutomatic,
            Notes = a.Notes
        }).ToList();

        var result = new AttendanceStatisticsDto
        {
            TotalDays = totalDays,
            PresentDays = presentDays,
            AbsentDays = absentDays,
            LateDays = lateDays,
            ExcusedDays = excusedDays,
            AttendancePercentage = attendancePercentage,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            LastAttendanceDate = allRecords.OrderByDescending(x => x.AttendanceDate).FirstOrDefault()?.AttendanceDate,
            RecentHistory = recentHistory
        };

        return RequestResult<AttendanceStatisticsDto>.Success(result);
    }

    private static int CalculateCurrentStreak(List<StudentAttendance> records)
    {
        if (!records.Any()) return 0;

        var sortedRecords = records.OrderByDescending(x => x.AttendanceDate).ToList();
        var streak = 0;
        var currentDate = DateTime.UtcNow.Date;

        foreach (var record in sortedRecords)
        {
            var recordDate = record.AttendanceDate.Date;
            
            // Check if this record is part of the current streak
            if (recordDate == currentDate || recordDate == currentDate.AddDays(-streak))
            {
                // Present, Late, or Excused count as attended
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

    private static int CalculateLongestStreak(List<StudentAttendance> records)
    {
        if (!records.Any()) return 0;

        var sortedRecords = records.OrderBy(x => x.AttendanceDate).ToList();
        var longestStreak = 0;
        var currentStreak = 0;

        foreach (var record in sortedRecords)
        {
            // Present, Late, or Excused count as attended
            if (record.Status == AttendanceStatus.Present || 
                record.Status == AttendanceStatus.Late || 
                record.Status == AttendanceStatus.Excused)
            {
                currentStreak++;
                longestStreak = Math.Max(longestStreak, currentStreak);
            }
            else
            {
                currentStreak = 0;
            }
        }

        return longestStreak;
    }
}

