using API.Application.Features.Teacher.Attendance.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Attendance.Commands;

public record MarkAttendanceCommand(long ClassId, MarkAttendanceRequest Request) : IRequest<RequestResult<bool>>;

public class MarkAttendanceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentAttendance> attendanceRepository,
    IRepository<Domain.Entities.General.Classes> classRepository)
    : RequestHandlerBase<MarkAttendanceCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(MarkAttendanceCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Verify class exists
        var classEntity = await classRepository.Get(x => x.ID == request.ClassId)
            .FirstOrDefaultAsync(cancellationToken);

        if (classEntity == null)
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Class not found");

        var now = DateTime.UtcNow;
        var attendanceDate = request.Request.AttendanceDate.Date;

        var recordsToAdd = new List<StudentAttendance>();
        var recordsToUpdate = new List<StudentAttendance>();

        foreach (var studentEntry in request.Request.Students)
        {
            if (!Enum.TryParse<AttendanceStatus>(studentEntry.Status, true, out var status))
            {
                return RequestResult<bool>.Failure(ErrorCode.ValidationError, $"Invalid attendance status: {studentEntry.Status}");
            }

            // Check if attendance already exists
            var existing = await attendanceRepository.Get(x =>
                x.StudentId == studentEntry.StudentId &&
                x.ClassId == request.ClassId &&
                x.AttendanceDate.Date == attendanceDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (existing != null)
            {
                // Update existing record
                existing.Status = status;
                existing.MarkedBy = teacherId;
                existing.MarkedAt = now;
                existing.IsAutomatic = false;
                existing.Notes = studentEntry.Notes;
                existing.UpdatedAt = now;
                recordsToUpdate.Add(existing);
            }
            else
            {
                // Create new record
                var attendance = new StudentAttendance
                {
                    StudentId = studentEntry.StudentId,
                    ClassId = request.ClassId,
                    AttendanceDate = attendanceDate,
                    Status = status,
                    MarkedBy = teacherId,
                    MarkedAt = now,
                    IsAutomatic = false,
                    Notes = studentEntry.Notes
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

        return RequestResult<bool>.Success(true, "Attendance marked successfully");
    }
}

