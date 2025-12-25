using API.Application.Features.Teacher.Exercises.DTOs;
using API.Application.Features.Teacher.Permissions.Services;
using API.Domain.Entities.Academic;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Exercises.Commands;

public record GradeExerciseSubmissionCommand(
    long SubmissionId,
    GradeExerciseSubmissionRequest Request) : IRequest<RequestResult<ExerciseSubmissionDto>>;

public class GradeExerciseSubmissionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ExerciseSubmissions> submissionRepository,
    IRepository<Exercises> exerciseRepository,
    TeacherPermissionService permissionService)
    : RequestHandlerBase<GradeExerciseSubmissionCommand, RequestResult<ExerciseSubmissionDto>>(parameters)
{
    public override async Task<RequestResult<ExerciseSubmissionDto>> Handle(
        GradeExerciseSubmissionCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var submission = await submissionRepository.Get(s =>
            s.ID == request.SubmissionId && !s.IsDeleted)
            .Include(s => s.Exercise)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission == null)
        {
            return RequestResult<ExerciseSubmissionDto>.Failure(
                ErrorCode.NotFound,
                "Submission not found");
        }

        if (submission.Exercise == null)
        {
            return RequestResult<ExerciseSubmissionDto>.Failure(
                ErrorCode.NotFound,
                "Exercise not found");
        }

        // Check permission to grade
        var canGrade = await permissionService.CanGradeAsync(
            teacherId,
            submission.Exercise.ClassId,
            submission.Exercise.SubjectId,
            cancellationToken);

        if (!canGrade)
        {
            return RequestResult<ExerciseSubmissionDto>.Failure(
                ErrorCode.Unauthorized,
                "You do not have permission to grade this submission");
        }

        // Validate score
        if (request.Request.Score < 0 || request.Request.Score > submission.Exercise.MaxScore)
        {
            return RequestResult<ExerciseSubmissionDto>.Failure(
                ErrorCode.ValidationError,
                $"Score must be between 0 and {submission.Exercise.MaxScore}");
        }

        // Update submission
        submission.Score = request.Request.Score;
        submission.Feedback = request.Request.Feedback;
        submission.Status = "Graded";
        submission.GradedBy = teacherId;
        submission.GradedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;
        submission.UpdatedBy = teacherId;

        submissionRepository.Update(submission);
        await submissionRepository.SaveChangesAsync(cancellationToken);

        // Return DTO
        var result = new ExerciseSubmissionDto
        {
            Id = submission.ID,
            ExerciseId = submission.ExerciseId,
            StudentId = submission.StudentId,
            StudentName = submission.Student?.Name ?? "Unknown",
            StudentEmail = submission.Student?.Email ?? "",
            SubmittedAt = submission.SubmittedAt,
            Content = submission.Content,
            Attachments = submission.Attachments,
            Status = submission.Status,
            Score = submission.Score,
            Feedback = submission.Feedback,
            GradedBy = submission.GradedBy,
            GraderName = submission.Grader?.Name,
            GradedAt = submission.GradedAt,
            IsLate = submission.Exercise.DueDate.HasValue &&
                     submission.SubmittedAt.HasValue &&
                     submission.SubmittedAt.Value > submission.Exercise.DueDate.Value
        };

        return RequestResult<ExerciseSubmissionDto>.Success(result);
    }
}

