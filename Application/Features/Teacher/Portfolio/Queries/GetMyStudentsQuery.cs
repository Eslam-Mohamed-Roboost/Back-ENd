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

        var studentDtos = new List<StudentPortfolioDto>();

        foreach (var student in students)
        {
            // Get class info
            var classInfo = await classRepository.Get(c => c.ID == student.ClassID!.Value)
                .FirstOrDefaultAsync(cancellationToken);

            // Get portfolio files for this student
            var portfolioFiles = await portfolioRepository.Get(p => p.StudentId == student.ID)
                .ToListAsync(cancellationToken);

            // Filter by subject if provided
            if (request.SubjectId.HasValue)
            {
                portfolioFiles = portfolioFiles.Where(p => p.SubjectId == request.SubjectId.Value).ToList();
            }

            var totalFiles = portfolioFiles.Count;
            var pendingFiles = portfolioFiles.Count(p => p.Status == "Pending");
            var reviewedFiles = portfolioFiles.Count(p => p.Status == "Reviewed");
            var needsRevisionFiles = portfolioFiles.Count(p => p.Status == "NeedsRevision");
            var lastSubmission = portfolioFiles.OrderByDescending(p => p.UploadedAt).FirstOrDefault();

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

            studentDtos.Add(new StudentPortfolioDto
            {
                StudentId = student.ID,
                StudentName = student.Name,
                Email = student.Email,
                ClassId = student.ClassID!.Value,
                ClassName = classInfo?.Name ?? "Unknown",
                TotalFiles = totalFiles,
                PendingFiles = pendingFiles,
                ReviewedFiles = reviewedFiles,
                NeedsRevisionFiles = needsRevisionFiles,
                LastSubmissionDate = lastSubmission?.UploadedAt,
                PortfolioStatus = portfolioStatus
            });
        }

        return RequestResult<List<StudentPortfolioDto>>.Success(
            studentDtos.OrderBy(s => s.ClassName).ThenBy(s => s.StudentName).ToList());
    }
}

