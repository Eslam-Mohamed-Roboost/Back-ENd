using API.Application.Features.Admin.Teachers.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;
using SubjectEntity = API.Domain.Entities.General.Subjects;

namespace API.Application.Features.Admin.Teachers.Commands;

public record AssignTeacherToClassCommand(
    long TeacherId,
    List<ClassSubjectAssignment> Assignments) : IRequest<RequestResult<TeacherAssignmentResponse>>;

public class AssignTeacherToClassCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<SubjectEntity> subjectRepository,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<AssignTeacherToClassCommand, RequestResult<TeacherAssignmentResponse>>(parameters)
{
    public override async Task<RequestResult<TeacherAssignmentResponse>> Handle(
        AssignTeacherToClassCommand request,
        CancellationToken cancellationToken)
    {
        // Validate teacher exists and is a teacher
        var teacher = await userRepository.Get(u => u.ID == request.TeacherId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null)
        {
            return RequestResult<TeacherAssignmentResponse>.Failure(ErrorCode.NotFound, "Teacher not found");
        }

        if (teacher.Role != ApplicationRole.Teacher)
        {
            return RequestResult<TeacherAssignmentResponse>.Failure(
                ErrorCode.ValidationError, 
                "User is not a teacher");
        }

        var response = new TeacherAssignmentResponse();
        var assignmentsToAdd = new List<TeacherClassAssignments>();

        foreach (var assignment in request.Assignments)
        {
            // Validate class exists
            var classExists = await classRepository.Get(c => c.ID == assignment.ClassId)
                .AnyAsync(cancellationToken);

            if (!classExists)
            {
                response.Errors.Add($"Class with ID {assignment.ClassId} not found");
                continue;
            }

            // Validate subject exists
            var subjectExists = await subjectRepository.Get(s => s.ID == assignment.SubjectId)
                .AnyAsync(cancellationToken);

            if (!subjectExists)
            {
                response.Errors.Add($"Subject with ID {assignment.SubjectId} not found");
                continue;
            }

            // Check if assignment already exists
            var existingAssignment = await assignmentRepository.Get(a =>
                a.TeacherId == request.TeacherId &&
                a.ClassId == assignment.ClassId &&
                a.SubjectId == assignment.SubjectId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingAssignment != null)
            {
                response.Errors.Add($"Teacher already assigned to class {assignment.ClassId} for subject {assignment.SubjectId}");
                continue;
            }

            // Create new assignment
            assignmentsToAdd.Add(new TeacherClassAssignments
            {
                TeacherId = request.TeacherId,
                ClassId = assignment.ClassId,
                SubjectId = assignment.SubjectId,
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (assignmentsToAdd.Any())
        {
            assignmentRepository.AddRange(assignmentsToAdd);
            await assignmentRepository.SaveChangesAsync();
            response.AssignmentsCreated = assignmentsToAdd.Count;
        }

        return RequestResult<TeacherAssignmentResponse>.Success(
            response, 
            $"Created {response.AssignmentsCreated} assignments with {response.Errors.Count} errors");
    }
}

