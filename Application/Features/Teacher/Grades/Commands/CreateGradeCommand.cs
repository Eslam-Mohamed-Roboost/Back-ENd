using API.Application.Features.Teacher.Grades.DTOs;
using API.Application.Features.Teacher.Permissions.Services;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Grades.Commands;

public record CreateGradeCommand(CreateGradeRequest Request) : IRequest<RequestResult<GradeDto>>;

public class CreateGradeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Grades> gradeRepository,
    IRepository<TeacherClassAssignments> assignmentRepository,
    TeacherPermissionService permissionService)
    : RequestHandlerBase<CreateGradeCommand, RequestResult<GradeDto>>(parameters)
{
    public override async Task<RequestResult<GradeDto>> Handle(
        CreateGradeCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Check permission to grade
        var canGrade = await permissionService.CanGradeAsync(
            teacherId,
            request.Request.ClassId,
            request.Request.SubjectId,
            cancellationToken);

        if (!canGrade)
        {
            return RequestResult<GradeDto>.Failure(
                ErrorCode.Unauthorized,
                "You do not have permission to create grades for this class/subject");
        }

        // Validate that either ExerciseId or ExaminationId is provided, but not both
        if (request.Request.ExerciseId.HasValue && request.Request.ExaminationId.HasValue)
        {
            return RequestResult<GradeDto>.Failure(
                ErrorCode.ValidationError,
                "Cannot assign grade to both exercise and examination");
        }

        // Calculate percentage and letter grade
        var percentage = (request.Request.Score / request.Request.MaxScore) * 100m;
        var letterGrade = CalculateLetterGrade(percentage);

        // Determine status - if approval is required, set to PendingApproval
        var canApprove = await permissionService.CanApproveGradesAsync(teacherId, cancellationToken);
        var status = canApprove ? "Approved" : "PendingApproval";

        // Create grade
        var grade = new Domain.Entities.Academic.Grades
        {
            StudentId = request.Request.StudentId,
            ClassId = request.Request.ClassId,
            SubjectId = request.Request.SubjectId,
            ExerciseId = request.Request.ExerciseId,
            ExaminationId = request.Request.ExaminationId,
            Score = request.Request.Score,
            MaxScore = request.Request.MaxScore,
            Percentage = percentage,
            LetterGrade = letterGrade,
            Term = request.Request.Term,
            Year = request.Request.Year,
            GradedBy = teacherId,
            GradedAt = DateTime.UtcNow,
            Status = status,
            Notes = request.Request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        gradeRepository.Add(grade);
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

