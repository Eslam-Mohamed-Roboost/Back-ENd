using API.Application.Features.Student.Attendance.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.General;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Attendance.Queries;

public record GetStudentAttendanceQuery(DateTime? StartDate = null, DateTime? EndDate = null) : IRequest<RequestResult<List<StudentAttendanceHistoryDto>>>;

public class GetStudentAttendanceQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentAttendance> attendanceRepository,
    IRepository<Classes> classRepository)
    : RequestHandlerBase<GetStudentAttendanceQuery, RequestResult<List<StudentAttendanceHistoryDto>>>(parameters)
{
    public override async Task<RequestResult<List<StudentAttendanceHistoryDto>>> Handle(GetStudentAttendanceQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var startDate = request.StartDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        var attendanceRecords = await attendanceRepository.Get(x =>
            x.StudentId == studentId &&
            x.AttendanceDate >= startDate &&
            x.AttendanceDate <= endDate)
            .OrderByDescending(x => x.AttendanceDate)
            .ToListAsync(cancellationToken);

        var classIds = attendanceRecords.Select(a => a.ClassId).Distinct().ToList();
        var classes = await classRepository.Get(x => classIds.Contains(x.ID))
            .ToDictionaryAsync(x => x.ID, cancellationToken);

        var result = attendanceRecords.Select(a => new StudentAttendanceHistoryDto
        {
            Date = a.AttendanceDate,
            ClassId = a.ClassId,
            ClassName = classes.GetValueOrDefault(a.ClassId)?.Name ?? "Unknown",
            Status = a.Status.ToString(),
            IsAutomatic = a.IsAutomatic,
            Notes = a.Notes
        }).ToList();

        return RequestResult<List<StudentAttendanceHistoryDto>>.Success(result);
    }
}

