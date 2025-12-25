using API.Application.Features.Teacher.Grades.DTOs;
using API.Application.Features.Teacher.Permissions.Services;
using API.Domain.Entities.Academic;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Grades.Commands;

public record UpdateGradeCommand(long GradeId, UpdateGradeRequest Request) : IRequest<RequestResult<GradeDto>>;

public class UpdateGradeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Grades> gradeRepository,
    TeacherPermissionService permissionService)
    : RequestHandlerBase<UpdateGradeCommand, RequestResult<GradeDto>>(parameters)
{
    public override async Task<RequestResult<GradeDto>> Handle(
        UpdateGradeCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var grade = await gradeRepository.Get(g =>
            g.ID == request.GradeId && !g.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (grade == null)
        {
            return RequestResult<GradeDto>.Failure(ErrorCode.NotFound, "Grade not found");
        }

        // Check permission to grade
        var canGrade = await permissionService.CanGradeAsync(
            teacherId,
            grade.ClassId,
            grade.SubjectId,
            cancellationToken);

        if (!canGrade)
        {
            return RequestResult<GradeDto>.Failure(
                ErrorCode.Unauthorized,
                "You do not have permission to update this grade");
        }

        // Can only update if status is Draft or PendingApproval
        if (grade.Status == "Approved")
        {
            return RequestResult<GradeDto>.Failure(
                ErrorCode.ValidationError,
                "Cannot update an approved grade");
        }

        // Update fields
        if (request.Request.Score.HasValue)
        {
            grade.Score = request.Request.Score.Value;
        }

        if (request.Request.MaxScore.HasValue)
        {
            grade.MaxScore = request.Request.MaxScore.Value;
        }

        if (request.Request.Term != null)
        {
            grade.Term = request.Request.Term;
        }

        if (request.Request.Year.HasValue)
        {
            grade.Year = request.Request.Year.Value;
        }

        if (request.Request.Notes != null)
        {
            grade.Notes = request.Request.Notes;
        }

        // Recalculate percentage and letter grade
        var percentage = (grade.Score / grade.MaxScore) * 100m;
        grade.Percentage = percentage;
        grade.LetterGrade = CalculateLetterGrade(percentage);

        grade.UpdatedAt = DateTime.UtcNow;
        grade.UpdatedBy = teacherId;

        gradeRepository.Update(grade);
        await gradeRepository.SaveChangesAsync();

        // Return DTO
        var result = new GradeDto
        {
            Id = grade.ID,
            StudentId = grade.StudentId,
            StudentName = grade.Student?.Name ?? "Unknown",
            ClassId = grade.ClassId,
            ClassName = grade.Class?.Name ?? "Unknown",
            SubjectId = grade.SubjectId,
            SubjectName = grade.Subject?.Name ?? "Unknown",
            ExerciseId = grade.ExerciseId,
            ExerciseTitle = grade.Exercise?.Title,
            ExaminationId = grade.ExaminationId,
            ExaminationTitle = grade.Examination?.Title,
            Score = grade.Score,
            MaxScore = grade.MaxScore,
            Percentage = grade.Percentage,
            LetterGrade = grade.LetterGrade,
            Term = grade.Term,
            Year = grade.Year,
            GradedBy = grade.GradedBy,
            GraderName = grade.Grader?.Name ?? "Unknown",
            GradedAt = grade.GradedAt,
            Status = grade.Status,
            ApprovedBy = grade.ApprovedBy,
            ApproverName = grade.Approver?.Name,
            ApprovedAt = grade.ApprovedAt,
            Notes = grade.Notes
        };

        return RequestResult<GradeDto>.Success(result);
    }

    private static string CalculateLetterGrade(decimal percentage)
    {
        return percentage switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }
}

