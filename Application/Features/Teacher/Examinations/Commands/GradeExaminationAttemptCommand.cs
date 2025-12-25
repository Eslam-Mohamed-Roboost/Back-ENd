using API.Application.Features.Teacher.Examinations.DTOs;
using API.Application.Features.Teacher.Permissions.Services;
using API.Domain.Entities.Academic;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Examinations.Commands;

public record GradeExaminationAttemptCommand(
    long AttemptId,
    GradeExaminationAttemptRequest Request) : IRequest<RequestResult<ExaminationAttemptDto>>;

public class GradeExaminationAttemptCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ExaminationAttempts> attemptRepository,
    IRepository<Domain.Entities.Academic.Examinations> examinationRepository,
    TeacherPermissionService permissionService)
    : RequestHandlerBase<GradeExaminationAttemptCommand, RequestResult<ExaminationAttemptDto>>(parameters)
{
    public override async Task<RequestResult<ExaminationAttemptDto>> Handle(
        GradeExaminationAttemptCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var attempt = await attemptRepository.Get(a =>
            a.ID == request.AttemptId && !a.IsDeleted)
            .Include(a => a.Examination)
            .Include(a => a.Student)
            .FirstOrDefaultAsync(cancellationToken);

        if (attempt == null)
        {
            return RequestResult<ExaminationAttemptDto>.Failure(
                ErrorCode.NotFound,
                "Attempt not found");
        }

        if (attempt.Examination == null)
        {
            return RequestResult<ExaminationAttemptDto>.Failure(
                ErrorCode.NotFound,
                "Examination not found");
        }

        // Check permission to grade
        var canGrade = await permissionService.CanGradeAsync(
            teacherId,
            attempt.Examination.ClassId,
            attempt.Examination.SubjectId,
            cancellationToken);

        if (!canGrade)
        {
            return RequestResult<ExaminationAttemptDto>.Failure(
                ErrorCode.Unauthorized,
                "You do not have permission to grade this attempt");
        }

        // Validate score
        if (request.Request.Score < 0 || request.Request.Score > attempt.Examination.MaxScore)
        {
            return RequestResult<ExaminationAttemptDto>.Failure(
                ErrorCode.ValidationError,
                $"Score must be between 0 and {attempt.Examination.MaxScore}");
        }

        // Update attempt
        attempt.Score = request.Request.Score;
        attempt.Status = "Graded";
        attempt.GradedBy = teacherId;
        attempt.GradedAt = DateTime.UtcNow;
        attempt.UpdatedAt = DateTime.UtcNow;
        attempt.UpdatedBy = teacherId;

        attemptRepository.Update(attempt);
        await attemptRepository.SaveChangesAsync();

        // Return DTO
        var result = new ExaminationAttemptDto
        {
            Id = attempt.ID,
            ExaminationId = attempt.ExaminationId,
            StudentId = attempt.StudentId,
            StudentName = attempt.Student?.Name ?? "Unknown",
            StudentEmail = attempt.Student?.Email ?? "",
            StartedAt = attempt.StartedAt,
            SubmittedAt = attempt.SubmittedAt,
            Answers = attempt.Answers,
            Score = attempt.Score,
            Status = attempt.Status,
            TimeSpent = attempt.TimeSpent,
            GradedBy = attempt.GradedBy,
            GraderName = attempt.Grader?.Name,
            GradedAt = attempt.GradedAt
        };

        return RequestResult<ExaminationAttemptDto>.Success(result);
    }
}

