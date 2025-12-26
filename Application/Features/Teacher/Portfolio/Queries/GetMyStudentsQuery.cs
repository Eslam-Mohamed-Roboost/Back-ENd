using API.Application.Features.Teacher.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Teacher.Portfolio.Queries;

public record GetMyStudentsQuery(long? SubjectId = null, long? ClassId = null) : IRequest<RequestResult<List<StudentPortfolioDto>>>;

public class GetMyStudentsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<TeacherClassAssignments> assignmentRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<PortfolioFiles> portfolioRepository)
    : RequestHandlerBase<GetMyStudentsQuery, RequestResult<List<StudentPortfolioDto>>>(parameters)
{
    public override async Task<RequestResult<List<StudentPortfolioDto>>> Handle(
        GetMyStudentsQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get classes assigned to this teacher
        var teacherAssignments = assignmentRepository.Get(a => a.TeacherId == teacherId);

        // Filter by subject if provided
        if (request.SubjectId.HasValue)
        {
            teacherAssignments = teacherAssignments.Where(a => a.SubjectId == request.SubjectId.Value);
        }

        // Filter by class if provided
        if (request.ClassId.HasValue)
        {
            teacherAssignments = teacherAssignments.Where(a => a.ClassId == request.ClassId.Value);
        }

        var assignedClassIds = await teacherAssignments
            .Select(a => a.ClassId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!assignedClassIds.Any())
        {
            return RequestResult<List<StudentPortfolioDto>>.Success(new List<StudentPortfolioDto>());
        }

        // Get students in these classes
        var students = await userRepository.Get(u => 
            u.Role == ApplicationRole.Student && 
            u.ClassID.HasValue && 
            assignedClassIds.Contains(u.ClassID.Value))
            .ToListAsync(cancellationToken);

        if (!students.Any())
        {
            return RequestResult<List<StudentPortfolioDto>>.Success(new List<StudentPortfolioDto>());
        }

        // Get all unique class IDs from students
        var studentClassIds = students
            .Where(s => s.ClassID.HasValue)
            .Select(s => s.ClassID!.Value)
            .Distinct()
            .ToList();

        // Fetch all classes in one query
        var classes = await classRepository.Get(c => studentClassIds.Contains(c.ID))
            .Select(c => new { c.ID, c.Name })
            .ToListAsync(cancellationToken);
        var classLookup = classes.ToDictionary(c => c.ID, c => c.Name);

        // Get all student IDs
        var studentIds = students.Select(s => s.ID).ToList();

        // Build portfolio files query for statistics
        var portfolioFilesQuery = portfolioRepository.Get(p => studentIds.Contains(p.StudentId));

        // Filter by subject if provided
        if (request.SubjectId.HasValue)
        {
            portfolioFilesQuery = portfolioFilesQuery.Where(p => p.SubjectId == request.SubjectId.Value);
        }

        // Get portfolio statistics grouped by student ID (executed in database)
        var portfolioStats = await portfolioFilesQuery
            .GroupBy(p => p.StudentId)
            .Select(g => new
            {
                StudentId = g.Key,
                TotalFiles = g.Count(),
                PendingFiles = g.Count(p => p.Status == "Pending"),
                ReviewedFiles = g.Count(p => p.Status == "Reviewed"),
                NeedsRevisionFiles = g.Count(p => p.Status == "NeedsRevision"),
                LastSubmissionDate = g.Max(p => (DateTime?)p.UploadedAt)
            })
            .ToListAsync(cancellationToken);

        var portfolioStatsLookup = portfolioStats.ToDictionary(s => s.StudentId);

        // Build student DTOs in memory
        var studentDtos = students.Select(student =>
        {
            var stats = portfolioStatsLookup.GetValueOrDefault(student.ID);

            var totalFiles = stats?.TotalFiles ?? 0;
            var pendingFiles = stats?.PendingFiles ?? 0;
            var reviewedFiles = stats?.ReviewedFiles ?? 0;
            var needsRevisionFiles = stats?.NeedsRevisionFiles ?? 0;
            var lastSubmissionDate = stats?.LastSubmissionDate;

            // Determine overall portfolio status
            string portfolioStatus = "Pending";
            if (needsRevisionFiles > 0)
            {
                portfolioStatus = "NeedsRevision";
            }
            else if (pendingFiles == 0 && reviewedFiles > 0)
            {
                portfolioStatus = "Reviewed";
            }

            return new StudentPortfolioDto
            {
                StudentId = student.ID,
                StudentName = student.Name,
                Email = student.Email,
                ClassId = student.ClassID!.Value,
                ClassName = classLookup.GetValueOrDefault(student.ClassID.Value, "Unknown"),
                TotalFiles = totalFiles,
                PendingFiles = pendingFiles,
                ReviewedFiles = reviewedFiles,
                NeedsRevisionFiles = needsRevisionFiles,
                LastSubmissionDate = lastSubmissionDate,
                PortfolioStatus = portfolioStatus
            };
        })
        .OrderBy(s => s.ClassName)
        .ThenBy(s => s.StudentName)
        .ToList();

        return RequestResult<List<StudentPortfolioDto>>.Success(studentDtos);
    }
}

