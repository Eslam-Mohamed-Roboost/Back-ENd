using API.Application.Features.Teacher.Attendance.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Attendance.Commands;

public record BulkMarkAttendanceCommand(BulkMarkAttendanceRequest Request) : IRequest<RequestResult<bool>>;

public class BulkMarkAttendanceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentAttendance> attendanceRepository,
    IRepository<Domain.Entities.General.Classes> classRepository)
    : RequestHandlerBase<BulkMarkAttendanceCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(BulkMarkAttendanceCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Verify class exists
        var classEntity = await classRepository.Get(x => x.ID == request.Request.ClassId)
            .FirstOrDefaultAsync(cancellationToken);

        if (classEntity == null)
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Class not found");

        if (!Enum.TryParse<AttendanceStatus>(request.Request.Status, true, out var status))
        {
            return RequestResult<bool>.Failure(ErrorCode.ValidationError, $"Invalid attendance status: {request.Request.Status}");
        }

        if (!request.Request.StudentIds.Any())
        {
            return RequestResult<bool>.Failure(ErrorCode.ValidationError, "No students provided");
        }

        var now = DateTime.UtcNow;
        var attendanceDate = request.Request.AttendanceDate.Date;

        // Get existing attendance records for these students on this date
        var existingRecords = await attendanceRepository.Get(x =>
            x.ClassId == request.Request.ClassId &&
            x.AttendanceDate.Date == attendanceDate &&
            request.Request.StudentIds.Contains(x.StudentId))
            .ToDictionaryAsync(x => x.StudentId, cancellationToken);

        var recordsToAdd = new List<StudentAttendance>();
        var recordsToUpdate = new List<StudentAttendance>();

        foreach (var studentId in request.Request.StudentIds)
        {
            if (existingRecords.TryGetValue(studentId, out var existing))
            {
                // Update existing record
                existing.Status = status;
                existing.MarkedBy = teacherId;
                existing.MarkedAt = now;
                existing.IsAutomatic = false;
                existing.Notes = request.Request.Notes;
                existing.UpdatedAt = now;
                recordsToUpdate.Add(existing);
            }
            else
            {
                // Create new record
                var attendance = new StudentAttendance
                {
                    StudentId = studentId,
                    ClassId = request.Request.ClassId,
                    AttendanceDate = attendanceDate,
                    Status = status,
                    MarkedBy = teacherId,
                    MarkedAt = now,
                    IsAutomatic = false,
                    Notes = request.Request.Notes
                };
                recordsToAdd.Add(attendance);
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

        return RequestResult<bool>.Success(true, $"Attendance marked for {request.Request.StudentIds.Count} students");
    }
}

