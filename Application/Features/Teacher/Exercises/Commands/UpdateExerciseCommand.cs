using API.Application.Features.Teacher.Exercises.DTOs;
using API.Domain.Entities.Academic;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Exercises.Commands;

public record UpdateExerciseCommand(long ExerciseId, UpdateExerciseRequest Request) : IRequest<RequestResult<ExerciseDto>>;

public class UpdateExerciseCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Exercises> exerciseRepository)
    : RequestHandlerBase<UpdateExerciseCommand, RequestResult<ExerciseDto>>(parameters)
{
    public override async Task<RequestResult<ExerciseDto>> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var exercise = await exerciseRepository.Get(e =>
            e.ID == request.ExerciseId && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (exercise == null)
        {
            return RequestResult<ExerciseDto>.Failure(ErrorCode.NotFound, "Exercise not found");
        }

        if (exercise.TeacherId != teacherId)
        {
            return RequestResult<ExerciseDto>.Failure(
                ErrorCode.Unauthorized,
                "You can only update your own exercises");
        }

        // Update fields
        if (!string.IsNullOrEmpty(request.Request.Title))
            exercise.Title = request.Request.Title;

        if (request.Request.Description != null)
            exercise.Description = request.Request.Description;

        if (!string.IsNullOrEmpty(request.Request.Type))
            exercise.Type = request.Request.Type;

        if (request.Request.DueDate.HasValue)
            exercise.DueDate = request.Request.DueDate;

        if (request.Request.MaxScore.HasValue)
            exercise.MaxScore = request.Request.MaxScore.Value;

        if (request.Request.Instructions != null)
            exercise.Instructions = request.Request.Instructions;

        if (request.Request.Attachments != null)
            exercise.Attachments = request.Request.Attachments;

        if (!string.IsNullOrEmpty(request.Request.Status))
            exercise.Status = request.Request.Status;

        exercise.UpdatedAt = DateTime.UtcNow;
        exercise.UpdatedBy = teacherId;

        exerciseRepository.Update(exercise);
        await exerciseRepository.SaveChangesAsync(cancellationToken);

        // Return DTO
        var result = new ExerciseDto
        {
            Id = exercise.ID,
            TeacherId = exercise.TeacherId,
            TeacherName = exercise.Teacher?.Name ?? "Unknown",
            ClassId = exercise.ClassId,
            ClassName = exercise.Class?.Name ?? "Unknown",
            SubjectId = exercise.SubjectId,
            SubjectName = exercise.Subject?.Name ?? "Unknown",
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
            SubmissionCount = 0, // Will be populated by query if needed
            GradedCount = 0
        };

        return RequestResult<ExerciseDto>.Success(result);
    }
}

