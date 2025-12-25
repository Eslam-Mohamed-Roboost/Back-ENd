using API.Application.Features.Teacher.Grades.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Grades.Queries;

public record GetGradesQuery(
    long? StudentId = null,
    long? ClassId = null,
    long? SubjectId = null,
    string? Term = null,
    int? Year = null,
    string? Status = null) : IRequest<RequestResult<List<GradeDto>>>;

public class GetGradesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Grades> gradeRepository,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetGradesQuery, RequestResult<List<GradeDto>>>(parameters)
{
    public override async Task<RequestResult<List<GradeDto>>> Handle(
        GetGradesQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get classes assigned to this teacher
        var assignedClassIds = await assignmentRepository.Get(a =>
            a.TeacherId == teacherId && !a.IsDeleted)
            .Select(a => a.ClassId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!assignedClassIds.Any())
        {
            return RequestResult<List<GradeDto>>.Success(new List<GradeDto>());
        }

        // Build query
        var query = gradeRepository.Get(g =>
            assignedClassIds.Contains(g.ClassId) &&
            !g.IsDeleted);

        // Apply filters
        if (request.StudentId.HasValue)
        {
            query = query.Where(g => g.StudentId == request.StudentId.Value);
        }

        if (request.ClassId.HasValue)
        {
            query = query.Where(g => g.ClassId == request.ClassId.Value);
        }

        if (request.SubjectId.HasValue)
        {
            query = query.Where(g => g.SubjectId == request.SubjectId.Value);
        }

        if (!string.IsNullOrEmpty(request.Term))
        {
            query = query.Where(g => g.Term == request.Term);
        }

        if (request.Year.HasValue)
        {
            query = query.Where(g => g.Year == request.Year.Value);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(g => g.Status == request.Status);
        }

        var grades = await query
            .OrderByDescending(g => g.GradedAt)
            .ToListAsync(cancellationToken);

        var result = grades.Select(g => new GradeDto
        {
            Id = g.ID,
            StudentId = g.StudentId,
            StudentName = g.Student?.Name ?? "Unknown",
            ClassId = g.ClassId,
            ClassName = g.Class?.Name ?? "Unknown",
            SubjectId = g.SubjectId,
            SubjectName = g.Subject?.Name ?? "Unknown",
            ExerciseId = g.ExerciseId,
            ExerciseTitle = g.Exercise?.Title,
            ExaminationId = g.ExaminationId,
            ExaminationTitle = g.Examination?.Title,
            Score = g.Score,
            MaxScore = g.MaxScore,
            Percentage = g.Percentage,
            LetterGrade = g.LetterGrade,
            Term = g.Term,
            Year = g.Year,
            GradedBy = g.GradedBy,
            GraderName = g.Grader?.Name ?? "Unknown",
            GradedAt = g.GradedAt,
            Status = g.Status,
            ApprovedBy = g.ApprovedBy,
            ApproverName = g.Approver?.Name,
            ApprovedAt = g.ApprovedAt,
            Notes = g.Notes
        }).ToList();

        return RequestResult<List<GradeDto>>.Success(result);
    }
}

