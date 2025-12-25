using API.Application.Features.Teacher.Grades.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Teacher.Grades.Queries;

public record GetGradeSummaryQuery(
    long ClassId,
    long SubjectId,
    string? Term = null,
    int? Year = null) : IRequest<RequestResult<GradeSummaryDto>>;

public class GetGradeSummaryQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Grades> gradeRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<TeacherClassAssignments> assignmentRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetGradeSummaryQuery, RequestResult<GradeSummaryDto>>(parameters)
{
    public override async Task<RequestResult<GradeSummaryDto>> Handle(
        GetGradeSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Verify teacher is assigned to this class/subject
        var isAssigned = await assignmentRepository.Get(a =>
            a.TeacherId == teacherId &&
            a.ClassId == request.ClassId &&
            a.SubjectId == request.SubjectId &&
            !a.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!isAssigned)
        {
            return RequestResult<GradeSummaryDto>.Failure(
                ErrorCode.Unauthorized,
                "You are not assigned to this class/subject");
        }

        // Get class info
        var classInfo = await classRepository.Get(c => c.ID == request.ClassId && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (classInfo == null)
        {
            return RequestResult<GradeSummaryDto>.Failure(ErrorCode.NotFound, "Class not found");
        }

        // Get total students in class
        var totalStudents = await userRepository.Get(u =>
            u.ClassID == request.ClassId &&
            u.Role == Domain.Enums.ApplicationRole.Student &&
            u.IsActive &&
            !u.IsDeleted)
            .CountAsync(cancellationToken);

        // Get grades query
        var gradesQuery = gradeRepository.Get(g =>
            g.ClassId == request.ClassId &&
            g.SubjectId == request.SubjectId &&
            !g.IsDeleted);

        if (!string.IsNullOrEmpty(request.Term))
        {
            gradesQuery = gradesQuery.Where(g => g.Term == request.Term);
        }

        if (request.Year.HasValue)
        {
            gradesQuery = gradesQuery.Where(g => g.Year == request.Year.Value);
        }

        var grades = await gradesQuery.ToListAsync(cancellationToken);

        var gradedStudents = grades.Select(g => g.StudentId).Distinct().Count();
        var approvedGrades = grades.Where(g => g.Status == "Approved").ToList();

        var averageScore = approvedGrades.Any()
            ? approvedGrades.Average(g => g.Percentage)
            : 0m;

        var highestScore = approvedGrades.Any()
            ? approvedGrades.Max(g => g.Percentage)
            : 0m;

        var lowestScore = approvedGrades.Any()
            ? approvedGrades.Min(g => g.Percentage)
            : 0m;

        // Grade distribution
        var gradeDistribution = approvedGrades
            .Where(g => !string.IsNullOrEmpty(g.LetterGrade))
            .GroupBy(g => g.LetterGrade!)
            .ToDictionary(g => g.Key, g => g.Count());

        var result = new GradeSummaryDto
        {
            ClassId = request.ClassId,
            ClassName = classInfo.Name,
            SubjectId = request.SubjectId,
            SubjectName = "", // Will be populated if needed
            TotalStudents = totalStudents,
            GradedStudents = gradedStudents,
            AverageScore = averageScore,
            HighestScore = highestScore,
            LowestScore = lowestScore,
            GradeDistribution = gradeDistribution,
            Term = request.Term,
            Year = request.Year ?? DateTime.Now.Year
        };

        return RequestResult<GradeSummaryDto>.Success(result);
    }
}

