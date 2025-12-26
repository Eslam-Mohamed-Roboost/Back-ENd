using API.Application.Features.Teacher.Attendance.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Attendance.Commands;

public record UpdateAttendanceCommand(long AttendanceId, UpdateAttendanceRequest Request) : IRequest<RequestResult<bool>>;

public class UpdateAttendanceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentAttendance> attendanceRepository)
    : RequestHandlerBase<UpdateAttendanceCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var attendance = await attendanceRepository.Get(x => x.ID == request.AttendanceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (attendance == null)
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Attendance record not found");

        if (!Enum.TryParse<AttendanceStatus>(request.Request.Status, true, out var status))
        {
            return RequestResult<bool>.Failure(ErrorCode.ValidationError, $"Invalid attendance status: {request.Request.Status}");
        }

        attendance.Status = status;
        attendance.MarkedBy = teacherId;
        attendance.MarkedAt = DateTime.UtcNow;
        attendance.IsAutomatic = false;
        attendance.Notes = request.Request.Notes;
        attendance.UpdatedAt = DateTime.UtcNow;

        attendanceRepository.Update(attendance);
        await attendanceRepository.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.Success(true, "Attendance updated successfully");
    }
}

