using API.Application.Services;
using API.Domain.Entities.Exercises;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ExercisesEntity = API.Domain.Entities.Exercises.Exercises;

namespace API.Application.Features.Teacher.Exercises.Commands;

public record GradeExerciseRequest(
    decimal Grade,
    string? Feedback);

public record GradeExerciseCommand(long SubmissionId, GradeExerciseRequest Request) : IRequest<RequestResult<bool>>;

public class GradeExerciseCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ExerciseSubmissions> submissionsRepository,
    IRepository<ExercisesEntity> exercisesRepository,
    INotificationService notificationService)
    : RequestHandlerBase<GradeExerciseCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        GradeExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;
        var req = request.Request;

        // Get submission with exercise
        var submission = await submissionsRepository
            .Get(s => s.ID == request.SubmissionId)
            .Include(s => s.Exercise)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Submission not found");
        }

        // Verify teacher owns this exercise
        if (submission.Exercise.TeacherId != teacherId)
        {
            return RequestResult<bool>.Failure(ErrorCode.ValidationError, "You can only grade your own exercises");
        }

        // Validate grade
        if (req.Grade < 0 || req.Grade > submission.Exercise.MaxPoints)
        {
            return RequestResult<bool>.Failure(
                ErrorCode.ValidationError,
                $"Grade must be between 0 and {submission.Exercise.MaxPoints}");
        }

        // Apply late penalty if applicable
        var finalGrade = req.Grade;
        if (submission.IsLate && submission.Exercise.LatePenaltyPercentage.HasValue)
        {
            var penalty = req.Grade * (submission.Exercise.LatePenaltyPercentage.Value / 100);
            finalGrade = req.Grade - penalty;
            finalGrade = Math.Max(0, finalGrade); // Don't go below 0
        }

        // Update submission
        submission.Grade = finalGrade;
        submission.Feedback = req.Feedback;
        submission.GradedBy = teacherId;
        submission.GradedAt = DateTime.UtcNow;
        submission.Status = SubmissionStatus.Approved; // Using Approved to mean "Graded"

        submissionsRepository.Update(submission);
        await submissionsRepository.SaveChangesAsync();

        // Notify student
        var gradePercentage = (finalGrade / submission.Exercise.MaxPoints) * 100;
        await notificationService.SendGradePublishedNotificationAsync(
            submission.StudentId,
            submission.Exercise.Title,
            gradePercentage,
            req.Feedback,
            submission.Exercise.ID,
            cancellationToken);

        return RequestResult<bool>.Success(true, "Exercise graded successfully");
    }
}

