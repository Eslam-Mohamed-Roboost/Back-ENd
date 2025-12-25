using API.Application.Services;
using API.Domain.Entities.Exercises;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserEntity = API.Domain.Entities.User;
using ExercisesEntity = API.Domain.Entities.Exercises.Exercises;

namespace API.Application.Features.Teacher.Exercises.Commands;

public record AssignExerciseRequest(
    string Title,
    string Description,
    ExerciseType Type,
    long? SubjectId,
    long? ClassId,
    DateTime? DueDate,
    int MaxPoints,
    string? Instructions,
    bool AllowLateSubmission,
    decimal? LatePenaltyPercentage);

public record AssignExerciseCommand(AssignExerciseRequest Request) : IRequest<RequestResult<long>>;

public class AssignExerciseCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ExercisesEntity> exercisesRepository,
    IRepository<UserEntity> userRepository,
    INotificationService notificationService)
    : RequestHandlerBase<AssignExerciseCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(
        AssignExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;
        var req = request.Request;

        // Validation
        if (string.IsNullOrWhiteSpace(req.Title))
        {
            return RequestResult<long>.Failure(ErrorCode.ValidationError, "Exercise title is required");
        }

        if (!req.SubjectId.HasValue && !req.ClassId.HasValue)
        {
            return RequestResult<long>.Failure(ErrorCode.ValidationError, "Either SubjectId or ClassId must be provided");
        }

        if (req.DueDate.HasValue && req.DueDate.Value < DateTime.UtcNow)
        {
            return RequestResult<long>.Failure(ErrorCode.ValidationError, "Due date cannot be in the past");
        }

        // Create exercise
        var exercise = new ExercisesEntity
        {
            Title = req.Title,
            Description = req.Description,
            Type = req.Type,
            TeacherId = teacherId,
            SubjectId = req.SubjectId,
            ClassId = req.ClassId,
            DueDate = req.DueDate,
            MaxPoints = req.MaxPoints,
            Instructions = req.Instructions,
            AllowLateSubmission = req.AllowLateSubmission,
            LatePenaltyPercentage = req.LatePenaltyPercentage,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };

        exercisesRepository.Add(exercise);
        await exercisesRepository.SaveChangesAsync();

        // Notify students
        List<UserEntity> studentsToNotify = new();

        if (req.ClassId.HasValue)
        {
            // Get all students in the class
            studentsToNotify = await userRepository
                .Get(u => u.ClassID == req.ClassId.Value && u.Role == ApplicationRole.Student && u.IsActive)
                .ToListAsync(cancellationToken);
        }

        foreach (var student in studentsToNotify)
        {
            await notificationService.SendExerciseAssignedNotificationAsync(
                student.ID,
                req.Title,
                req.DueDate,
                exercise.ID,
                cancellationToken);
        }

        return RequestResult<long>.Success(exercise.ID, $"Exercise assigned successfully to {studentsToNotify.Count} students");
    }
}

