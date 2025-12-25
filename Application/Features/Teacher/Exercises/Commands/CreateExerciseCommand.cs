using API.Application.Features.Teacher.Exercises.DTOs;
using API.Application.Features.Teacher.Permissions.Services;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Teacher.Exercises.Commands;

public record CreateExerciseCommand(CreateExerciseRequest Request) : IRequest<RequestResult<ExerciseDto>>;

public class CreateExerciseCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Exercises> exerciseRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<TeacherClassAssignments> assignmentRepository,
    TeacherPermissionService permissionService)
    : RequestHandlerBase<CreateExerciseCommand, RequestResult<ExerciseDto>>(parameters)
{
    public override async Task<RequestResult<ExerciseDto>> Handle(
        CreateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Check permission
        var canCreate = await permissionService.CanCreateExercisesAsync(
            teacherId,
            request.Request.ClassId,
            request.Request.SubjectId,
            cancellationToken);

        if (!canCreate)
        {
            return RequestResult<ExerciseDto>.Failure(
                ErrorCode.Unauthorized,
                "You do not have permission to create exercises for this class/subject");
        }

        // Validate class exists
        var classExists = await classRepository.Get(c =>
            c.ID == request.Request.ClassId && !c.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!classExists)
        {
            return RequestResult<ExerciseDto>.Failure(
                ErrorCode.NotFound,
                "Class not found");
        }

        // Create exercise
        var exercise = new Exercises
        {
            TeacherId = teacherId,
            ClassId = request.Request.ClassId,
            SubjectId = request.Request.SubjectId,
            Title = request.Request.Title,
            Description = request.Request.Description,
            Type = request.Request.Type,
            DueDate = request.Request.DueDate,
            MaxScore = request.Request.MaxScore,
            Instructions = request.Request.Instructions,
            Attachments = request.Request.Attachments,
            Status = request.Request.Status,
            CreatedAt = DateTime.UtcNow
        };

        exerciseRepository.Add(exercise);
        await exerciseRepository.SaveChangesAsync(cancellationToken);

        // Return DTO
        var result = new ExerciseDto
        {
            Id = exercise.ID,
            TeacherId = exercise.TeacherId,
            TeacherName = "Current Teacher", // Will be populated by query if needed
            ClassId = exercise.ClassId,
            ClassName = "", // Will be populated by query if needed
            SubjectId = exercise.SubjectId,
            SubjectName = "", // Will be populated by query if needed
            Title = exercise.Title,
            Description = exercise.Description,
            Type = exercise.Type,
            DueDate = exercise.DueDate,
            MaxScore = exercise.MaxScore,
            Instructions = exercise.Instructions,
            Attachments = exercise.Attachments,
            Status = exercise.Status,
            CreatedAt = exercise.CreatedAt,
            UpdatedAt = exercise.UpdatedAt,
            SubmissionCount = 0,
            GradedCount = 0
        };

        return RequestResult<ExerciseDto>.Success(result);
    }
}

