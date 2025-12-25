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

        // Get classes with student counts
        var classes = await classRepository.Get(c =>
            assignedClassIds.Contains(c.ID) && !c.IsDeleted)
            .Select(c => new
            {
                Class = c,
                StudentCount = userRepository.Get(u =>
                    u.ClassID == c.ID &&
                    u.Role == ApplicationRole.Student &&
                    u.IsActive &&
                    !u.IsDeleted).Count(),
                Subjects = assignmentRepository.Get(a =>
                    a.TeacherId == teacherId &&
                    a.ClassId == c.ID &&
                    !a.IsDeleted)
                    .Select(a => new ClassSubjectInfo
                    {
                        SubjectId = a.SubjectId,
                        SubjectName = a.Subject != null ? a.Subject.Name : "Unknown"
                    })
                    .Distinct()
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var result = classes.Select(c => new TeacherClassDto
        {
            Id = c.Class.ID,
            Name = c.Class.Name,
            Grade = c.Class.Grade,
            StudentCount = c.StudentCount,
            Subjects = c.Subjects
        }).OrderBy(c => c.Grade).ThenBy(c => c.Name).ToList();

        return RequestResult<List<TeacherClassDto>>.Success(result);
    }
}

