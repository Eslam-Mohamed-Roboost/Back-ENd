using API.Application.Features.Teacher.Classes.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Teacher.Classes.Queries;

public record GetClassStudentsQuery(long ClassId) : IRequest<RequestResult<List<ClassStudentDto>>>;

public class GetClassStudentsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherClassAssignments> assignmentRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetClassStudentsQuery, RequestResult<List<ClassStudentDto>>>(parameters)
{
    public override async Task<RequestResult<List<ClassStudentDto>>> Handle(
        GetClassStudentsQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Verify teacher is assigned to this class
        var isAssigned = await assignmentRepository.Get(a =>
            a.TeacherId == teacherId &&
            a.ClassId == request.ClassId &&
            !a.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!isAssigned)
        {
            return RequestResult<List<ClassStudentDto>>.Failure(
                ErrorCode.Unauthorized,
                "You are not assigned to this class");
        }

        // Get class info
        var classInfo = await classRepository.Get(c => c.ID == request.ClassId && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (classInfo == null)
        {
            return RequestResult<List<ClassStudentDto>>.Failure(
                ErrorCode.NotFound,
                "Class not found");
        }

        // Get students in this class
        var students = await userRepository.Get(u =>
            u.ClassID == request.ClassId &&
            u.Role == ApplicationRole.Student &&
            !u.IsDeleted)
            .Select(u => new ClassStudentDto
            {
                Id = u.ID,
                Name = u.Name,
                Email = u.Email,
                ClassId = u.ClassID!.Value,
                ClassName = classInfo.Name,
                IsActive = u.IsActive,
                LastLogin = u.LastLogin
            })
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return RequestResult<List<ClassStudentDto>>.Success(students);
    }
}

