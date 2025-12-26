using API.Domain.Entities.Academic;
using API.Domain.Entities;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Attendance.Commands;

public record ProcessAutomaticAttendanceCommand(DateTime Date, long ClassId) : IRequest<RequestResult<int>>;

public class ProcessAutomaticAttendanceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentAttendance> attendanceRepository,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Portfolio.PortfolioFiles> portfolioFilesRepository,
    IRepository<Domain.Entities.Missions.StudentMissionProgress> missionProgressRepository)
    : RequestHandlerBase<ProcessAutomaticAttendanceCommand, RequestResult<int>>(parameters)
{
    public override async Task<RequestResult<int>> Handle(ProcessAutomaticAttendanceCommand request, CancellationToken cancellationToken)
    {
        var attendanceDate = request.Date.Date;
        var classStartTime = attendanceDate.AddHours(8); // Assuming class starts at 8 AM
        var classEndTime = attendanceDate.AddHours(15); // Assuming class ends at 3 PM

        // Get all students in the class
        var students = await userRepository.Get(x =>
            x.Role == ApplicationRole.Student &&
            x.ClassID == request.ClassId)
            .ToListAsync(cancellationToken);

        if (!students.Any())
        {
            return RequestResult<int>.Success(0, "No students found in class");
        }

        var studentIds = students.Select(s => s.ID).ToList();

        // Get existing attendance records for this date
        var existingRecords = await attendanceRepository.Get(x =>
            x.ClassId == request.ClassId &&
            x.AttendanceDate.Date == attendanceDate)
            .ToDictionaryAsync(x => x.StudentId, cancellationToken);

        // Check for student activity during class time
        // Activity includes: portfolio uploads, mission progress updates, login activity
        var portfolioActivity = await portfolioFilesRepository.Get(x =>
            studentIds.Contains(x.StudentId) &&
            x.UploadedAt >= classStartTime &&
            x.UploadedAt <= classEndTime)
            .Select(x => x.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var missionActivity = await missionProgressRepository.Get(x =>
            studentIds.Contains(x.StudentId) &&
            x.UpdatedAt >= classStartTime &&
            x.UpdatedAt <= classEndTime)
            .Select(x => x.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var activeStudentIds = portfolioActivity.Union(missionActivity).Distinct().ToList();

        var recordsToAdd = new List<StudentAttendance>();
        var recordsToUpdate = new List<StudentAttendance>();
        var processedCount = 0;

        foreach (var student in students)
        {
            var hasActivity = activeStudentIds.Contains(student.ID);
            var status = hasActivity ? AttendanceStatus.Present : AttendanceStatus.Absent;

            if (existingRecords.TryGetValue(student.ID, out var existing))
            {
                // Only update if it was automatically marked and we have new information
                if (existing.IsAutomatic)
                {
                    existing.Status = status;
                    existing.MarkedAt = DateTime.UtcNow;
                    existing.UpdatedAt = DateTime.UtcNow;
                    recordsToUpdate.Add(existing);
                    processedCount++;
                }
            }
            else
            {
                // Create new automatic attendance record
                var attendance = new StudentAttendance
                {
                    StudentId = student.ID,
                    ClassId = request.ClassId,
                    AttendanceDate = attendanceDate,
                    Status = status,
                    MarkedBy = null, // Automatic, no teacher
                    MarkedAt = DateTime.UtcNow,
                    IsAutomatic = true,
                    Notes = hasActivity ? "Auto-marked based on activity" : "Auto-marked as absent (no activity detected)"
                };
                recordsToAdd.Add(attendance);
                processedCount++;
            }
        }

        foreach (var record in recordsToAdd)
        {
            attendanceRepository.Add(record);
        }

        foreach (var record in recordsToUpdate)
        {
            attendanceRepository.Update(record);
        }

        await attendanceRepository.SaveChangesAsync(cancellationToken);

        return RequestResult<int>.Success(processedCount, $"Processed automatic attendance for {processedCount} students");
    }
}

