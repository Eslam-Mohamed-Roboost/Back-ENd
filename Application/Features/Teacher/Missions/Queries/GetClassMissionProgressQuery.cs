using API.Application.Features.Admin.Missions.DTOs;
using API.Domain.Entities.Missions;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;
using MissionEntity = API.Domain.Entities.Missions.Missions;

namespace API.Application.Features.Teacher.Missions.Queries;

public record GetClassMissionProgressQuery : IRequest<RequestResult<List<ClassMissionProgressDto>>>;

public class GetClassMissionProgressQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<TeacherClassAssignments> assignmentRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<StudentMissionProgress> missionProgressRepository,
    IRepository<MissionEntity> missionRepository)
    : RequestHandlerBase<GetClassMissionProgressQuery, RequestResult<List<ClassMissionProgressDto>>>(parameters)
{
    public override async Task<RequestResult<List<ClassMissionProgressDto>>> Handle(
        GetClassMissionProgressQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get classes assigned to this teacher
        var assignedClassIds = await assignmentRepository.Get(a => a.TeacherId == teacherId)
            .Select(a => a.ClassId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!assignedClassIds.Any())
        {
            return RequestResult<List<ClassMissionProgressDto>>.Success(new List<ClassMissionProgressDto>());
        }

        var totalMissions = await missionRepository.Get().CountAsync(cancellationToken);
        var classProgressList = new List<ClassMissionProgressDto>();

        foreach (var classId in assignedClassIds)
        {
            var classInfo = await classRepository.Get(c => c.ID == classId)
                .FirstOrDefaultAsync(cancellationToken);

            if (classInfo == null) continue;

            var studentsInClass = await userRepository.Get(u => 
                u.Role == ApplicationRole.Student && 
                u.ClassID == classId)
                .ToListAsync(cancellationToken);

            var studentProgressList = new List<StudentMissionProgressDto>();

            foreach (var student in studentsInClass)
            {
                var studentProgress = await missionProgressRepository.Get(mp => mp.StudentId == student.ID)
                    .ToListAsync(cancellationToken);

                var missionsCompleted = studentProgress.Count(mp => mp.Status == ProgressStatus.Completed);
                var completionRate = totalMissions > 0 ? (decimal)missionsCompleted / totalMissions * 100 : 0;
                var lastActivity = studentProgress.OrderByDescending(mp => mp.UpdatedAt).FirstOrDefault()?.UpdatedAt;

                studentProgressList.Add(new StudentMissionProgressDto
                {
                    StudentId = student.ID,
                    StudentName = student.Name,
                    MissionsCompleted = missionsCompleted,
                    TotalMissions = totalMissions,
                    CompletionRate = completionRate,
                    LastActivityDate = lastActivity
                });
            }

            var avgCompletionRate = studentProgressList.Any()
                ? studentProgressList.Average(s => s.CompletionRate)
                : 0;

            classProgressList.Add(new ClassMissionProgressDto
            {
                ClassId = classInfo.ID,
                ClassName = classInfo.Name,
                Grade = classInfo.Grade,
                TotalStudents = studentsInClass.Count,
                AverageCompletionRate = avgCompletionRate,
                StudentProgress = studentProgressList.OrderBy(s => s.StudentName).ToList()
            });
        }

        return RequestResult<List<ClassMissionProgressDto>>.Success(
            classProgressList.OrderBy(c => c.ClassName).ToList());
    }
}

