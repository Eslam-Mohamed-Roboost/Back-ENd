using API.Application.Features.Teacher.Grades.DTOs;
using API.Application.Features.Teacher.Permissions.Services;
using API.Domain.Entities.Academic;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Grades.Commands;

public record ApproveGradeCommand(long GradeId, GradeApprovalRequest Request) : IRequest<RequestResult<GradeDto>>;

public class ApproveGradeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Grades> gradeRepository,
    TeacherPermissionService permissionService)
    : RequestHandlerBase<ApproveGradeCommand, RequestResult<GradeDto>>(parameters)
{
    public override async Task<RequestResult<GradeDto>> Handle(
        ApproveGradeCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Check permission to approve
        var canApprove = await permissionService.CanApproveGradesAsync(teacherId, cancellationToken);

        if (!canApprove)
        {
            return RequestResult<GradeDto>.Failure(
                ErrorCode.Unauthorized,
                "You do not have permission to approve grades");
        }

        var grade = await gradeRepository.Get(g =>
            g.ID == request.GradeId && !g.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (grade == null)
        {
            return RequestResult<GradeDto>.Failure(ErrorCode.NotFound, "Grade not found");
        }

        if (request.Request.Approve)
        {
            // Approve grade
            if (grade.Status != "PendingApproval")
            {
                return RequestResult<GradeDto>.Failure(
                    ErrorCode.ValidationError,
                    "Only grades with PendingApproval status can be approved");
            }

            grade.Status = "Approved";
            grade.ApprovedBy = teacherId;
            grade.ApprovedAt = DateTime.UtcNow;
        }
        else
        {
            // Reject grade
            if (grade.Status != "PendingApproval")
            {
                return RequestResult<GradeDto>.Failure(
                    ErrorCode.ValidationError,
                    "Only grades with PendingApproval status can be rejected");
            }

            grade.Status = "Rejected";
            grade.ApprovedBy = teacherId;
            grade.ApprovedAt = DateTime.UtcNow;
        }

        if (!string.IsNullOrEmpty(request.Request.Notes))
        {
            grade.Notes = string.IsNullOrEmpty(grade.Notes)
                ? request.Request.Notes
                : $"{grade.Notes}\n{request.Request.Notes}";
        }

        grade.UpdatedAt = DateTime.UtcNow;
        grade.UpdatedBy = teacherId;

        gradeRepository.Update(grade);
        await gradeRepository.SaveChangesAsync(cancellationToken);

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
}

