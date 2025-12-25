using API.Application.Features.Teacher.Examinations.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Examinations.Queries;

public record GetExaminationsQuery(
    long? ClassId = null,
    long? SubjectId = null,
    string? Type = null,
    string? Status = null) : IRequest<RequestResult<List<ExaminationDto>>>;

public class GetExaminationsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Examinations> examinationRepository,
    IRepository<ExaminationAttempts> attemptRepository,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetExaminationsQuery, RequestResult<List<ExaminationDto>>>(parameters)
{
    public override async Task<RequestResult<List<ExaminationDto>>> Handle(
        GetExaminationsQuery request,
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
            return RequestResult<List<ExaminationDto>>.Success(new List<ExaminationDto>());
        }

        // Build query
        var query = examinationRepository.Get(e =>
            e.TeacherId == teacherId &&
            assignedClassIds.Contains(e.ClassId) &&
            !e.IsDeleted);

        // Apply filters
        if (request.ClassId.HasValue)
        {
            query = query.Where(e => e.ClassId == request.ClassId.Value);
        }

        if (request.SubjectId.HasValue)
        {
            query = query.Where(e => e.SubjectId == request.SubjectId.Value);
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            query = query.Where(e => e.Type == request.Type);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(e => e.Status == request.Status);
        }

        var examinations = await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);

        var result = new List<ExaminationDto>();

        foreach (var examination in examinations)
        {
            var attemptCount = await attemptRepository.Get(a =>
                a.ExaminationId == examination.ID && !a.IsDeleted)
                .CountAsync(cancellationToken);

            var gradedCount = await attemptRepository.Get(a =>
                a.ExaminationId == examination.ID &&
                a.Status == "Graded" &&
                !a.IsDeleted)
                .CountAsync(cancellationToken);

            result.Add(new ExaminationDto
            {
                Id = examination.ID,
                TeacherId = examination.TeacherId,
                TeacherName = examination.Teacher?.Name ?? "Unknown",
                ClassId = examination.ClassId,
                ClassName = examination.Class?.Name ?? "Unknown",
                SubjectId = examination.SubjectId,
                SubjectName = examination.Subject?.Name ?? "Unknown",
                Title = examination.Title,
                Description = examination.Description,
                Type = examination.Type,
                ScheduledDate = examination.ScheduledDate,
                Duration = examination.Duration,
                MaxScore = examination.MaxScore,
                Instructions = examination.Instructions,
                Questions = examination.Questions,
                Status = examination.Status,
                CreatedAt = examination.CreatedAt,
                UpdatedAt = examination.UpdatedAt,
                AttemptCount = attemptCount,
                GradedCount = gradedCount
            });
        }

        return RequestResult<List<ExaminationDto>>.Success(result);
    }
}

