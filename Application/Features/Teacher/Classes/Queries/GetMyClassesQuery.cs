using API.Application.Features.Teacher.Classes.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Teacher.Classes.Queries;

public record GetMyClassesQuery : IRequest<RequestResult<List<TeacherClassDto>>>;

public class GetMyClassesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherClassAssignments> assignmentRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetMyClassesQuery, RequestResult<List<TeacherClassDto>>>(parameters)
{
    public override async Task<RequestResult<List<TeacherClassDto>>> Handle(
        GetMyClassesQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get all classes assigned to this teacher
        var assignedClassIds = await assignmentRepository.Get(a =>
            a.TeacherId == teacherId && !a.IsDeleted)
            .Select(a => a.ClassId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!assignedClassIds.Any())
        {
            return RequestResult<List<TeacherClassDto>>.Success(new List<TeacherClassDto>());
        }

        // Get classes first
        var classes = await classRepository.Get(c =>
            assignedClassIds.Contains(c.ID) && !c.IsDeleted)
            .ToListAsync(cancellationToken);

        // Get all assignments for this teacher and these classes
        var classIds = classes.Select(c => c.ID).ToList();
        var assignments = await assignmentRepository.Get(a =>
            a.TeacherId == teacherId &&
            classIds.Contains(a.ClassId) &&
            !a.IsDeleted)
            .Include(a => a.Subject)
            .ToListAsync(cancellationToken);

        // Get student counts for each class
        var studentCounts = await userRepository.Get(u =>
            u.ClassID.HasValue &&
            classIds.Contains(u.ClassID.Value) &&
            u.Role == ApplicationRole.Student &&
            u.IsActive &&
            !u.IsDeleted)
            .GroupBy(u => u.ClassID)
            .Select(g => new { ClassId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // Build result
        var result = classes.Select(c =>
        {
            var studentCount = studentCounts.FirstOrDefault(sc => sc.ClassId == c.ID)?.Count ?? 0;
            var classAssignments = assignments.Where(a => a.ClassId == c.ID).ToList();
            
            return new TeacherClassDto
            {
                Id = c.ID,
                Name = c.Name,
                Grade = c.Grade,
                StudentCount = studentCount,
                Subjects = classAssignments
                    .GroupBy(a => a.SubjectId)
                    .Select(g => new ClassSubjectInfo
                    {
                        SubjectId = g.Key,
                        SubjectName = g.First().Subject?.Name ?? "Unknown"
                    })
                    .ToList()
            };
        })
        .OrderBy(c => c.Grade)
        .ThenBy(c => c.Name)
        .ToList();

        return RequestResult<List<TeacherClassDto>>.Success(result);
    }
}

