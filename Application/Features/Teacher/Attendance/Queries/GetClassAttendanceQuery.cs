using API.Application.Features.Teacher.Attendance.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Attendance.Queries;

public record GetClassAttendanceQuery(long ClassId, DateTime Date) : IRequest<RequestResult<ClassAttendanceDto>>;

public class GetClassAttendanceQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentAttendance> attendanceRepository,
    IRepository<Domain.Entities.General.Classes> classRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetClassAttendanceQuery, RequestResult<ClassAttendanceDto>>(parameters)
{
    public override async Task<RequestResult<ClassAttendanceDto>> Handle(GetClassAttendanceQuery request, CancellationToken cancellationToken)
    {
        var classEntity = await classRepository.Get(x => x.ID == request.ClassId)
            .FirstOrDefaultAsync(cancellationToken);

        if (classEntity == null)
            return RequestResult<ClassAttendanceDto>.Failure(ErrorCode.NotFound, "Class not found");

        // Get all students in the class
        var students = await userRepository.Get(x =>
            x.Role == ApplicationRole.Student &&
            x.ClassID == request.ClassId)
            .ToListAsync(cancellationToken);

        var studentIds = students.Select(s => s.ID).ToList();
        var studentMap = students.ToDictionary(s => s.ID);

        // Get attendance records for this class and date
        var attendanceRecords = await attendanceRepository.Get(x =>
            x.ClassId == request.ClassId &&
            x.AttendanceDate.Date == request.Date.Date)
            .ToDictionaryAsync(x => x.StudentId, cancellationToken);

        // Get marked by teacher names
        var teacherIds = attendanceRecords.Values
            .Where(a => a.MarkedBy.HasValue)
            .Select(a => a.MarkedBy!.Value)
            .Distinct()
            .ToList();
        
        var teachers = teacherIds.Any() 
            ? await userRepository.Get(x => teacherIds.Contains(x.ID))
                .ToDictionaryAsync(x => x.ID, cancellationToken)
            : new Dictionary<long, Domain.Entities.User>();

        // Create attendance DTOs for all students (include students without records as "Absent" or "Pending")
        var attendanceDtos = students.Select(student =>
        {
            var attendance = attendanceRecords.GetValueOrDefault(student.ID);
            
            return new AttendanceDto
            {
                Id = attendance?.ID ?? 0,
                StudentId = student.ID,
                StudentName = student.Name,
                ClassId = request.ClassId,
                ClassName = classEntity.Name,
                AttendanceDate = request.Date.Date,
                Status = attendance != null ? attendance.Status.ToString() : "Absent", // Default to Absent if not marked
                MarkedBy = attendance?.MarkedBy,
                MarkedByName = attendance?.MarkedBy.HasValue == true 
                    ? teachers.GetValueOrDefault(attendance.MarkedBy!.Value)?.Name 
                    : null,
                MarkedAt = attendance?.MarkedAt,
                IsAutomatic = attendance?.IsAutomatic ?? false,
                Notes = attendance?.Notes
            };
        }).ToList();

        var presentCount = attendanceDtos.Count(a => a.Status == "Present");
        var absentCount = attendanceDtos.Count(a => a.Status == "Absent");
        var lateCount = attendanceDtos.Count(a => a.Status == "Late");
        var excusedCount = attendanceDtos.Count(a => a.Status == "Excused");

        var result = new ClassAttendanceDto
        {
            Date = request.Date.Date,
            ClassId = request.ClassId,
            ClassName = classEntity.Name,
            Students = attendanceDtos,
            PresentCount = presentCount,
            AbsentCount = absentCount,
            LateCount = lateCount,
            ExcusedCount = excusedCount,
            TotalStudents = attendanceDtos.Count
        };

        return RequestResult<ClassAttendanceDto>.Success(result);
    }
}

