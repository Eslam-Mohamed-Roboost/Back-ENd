using API.Application.Services;
using API.Domain.Entities.Exercises;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserEntity = API.Domain.Entities.User;
using ExercisesEntity = API.Domain.Entities.Exercises.Exercises;

namespace API.Application.Features.Student.Exercises.Commands;

public record SubmitExerciseRequest(
    string? SubmissionText,
    string? FileUrl);

public record SubmitExerciseCommand(long ExerciseId, SubmitExerciseRequest Request) : IRequest<RequestResult<long>>;

public class SubmitExerciseCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ExercisesEntity> exercisesRepository,
    IRepository<ExerciseSubmissions> submissionsRepository,
    IRepository<UserEntity> userRepository,
    INotificationService notificationService)
    : RequestHandlerBase<SubmitExerciseCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(
        SubmitExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var req = request.Request;

        // Get exercise
        var exercise = await exercisesRepository
            .Get(e => e.ID == request.ExerciseId && e.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (exercise == null)
        {
            return RequestResult<long>.Failure(ErrorCode.NotFound, "Exercise not found");
        }

        // Check for existing submission
        var existingSubmission = await submissionsRepository
            .Get(s => s.ExerciseId == request.ExerciseId && s.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSubmission != null)
        {
            return RequestResult<long>.Failure(ErrorCode.ValidationError, "You have already submitted this exercise");
        }

        // Validate submission content
        if (string.IsNullOrWhiteSpace(req.SubmissionText) && string.IsNullOrWhiteSpace(req.FileUrl))
        {
            return RequestResult<long>.Failure(ErrorCode.ValidationError, "Submission must include text or file");
        }

        // Check deadline
        var isLate = false;
        if (exercise.DueDate.HasValue && DateTime.UtcNow > exercise.DueDate.Value)
        {
            if (!exercise.AllowLateSubmission)
            {
                return RequestResult<long>.Failure(ErrorCode.ValidationError, "Submission deadline has passed");
            }
            isLate = true;
        }

        // Create submission
        var submission = new ExerciseSubmissions
        {
            ExerciseId = request.ExerciseId,
            StudentId = studentId,
            SubmissionText = req.SubmissionText,
            FileUrl = req.FileUrl,
            SubmittedAt = DateTime.UtcNow,
            IsLate = isLate,
            Status = SubmissionStatus.Pending
        };

        submissionsRepository.Add(submission);
        await submissionsRepository.SaveChangesAsync();

        // Notify teacher
        var student = await userRepository.Get(u => u.ID == studentId).FirstOrDefaultAsync(cancellationToken);
        var studentName = student?.Name ?? "A student";

        await notificationService.SendExerciseSubmissionNotificationAsync(
            exercise.TeacherId,
            studentId,
            studentName,
            exercise.Title,
            submission.ID,
            cancellationToken);

        return RequestResult<long>.Success(submission.ID, "Exercise submitted successfully");
    }
}

