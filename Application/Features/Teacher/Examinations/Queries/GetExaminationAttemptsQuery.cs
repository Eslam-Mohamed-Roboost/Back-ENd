using API.Application.Features.Teacher.Examinations.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Examinations.Queries;

public record GetExaminationAttemptsQuery(
    long ExaminationId,
    string? Status = null) : IRequest<RequestResult<List<ExaminationAttemptDto>>>;

public class GetExaminationAttemptsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Examinations> examinationRepository,
    IRepository<ExaminationAttempts> attemptRepository,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetExaminationAttemptsQuery, RequestResult<List<ExaminationAttemptDto>>>(parameters)
{
    public override async Task<RequestResult<List<ExaminationAttemptDto>>> Handle(
        GetExaminationAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var examination = await examinationRepository.Get(e =>
            e.ID == request.ExaminationId && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (examination == null)
        {
            return RequestResult<List<ExaminationAttemptDto>>.Failure(
                ErrorCode.NotFound,
                "Examination not found");
        }

        if (examination.TeacherId != teacherId)
        {
            var isAssigned = await assignmentRepository.Get(a =>
                a.TeacherId == teacherId &&
                a.ClassId == examination.ClassId &&
                a.SubjectId == examination.SubjectId &&
                !a.IsDeleted)
                .AnyAsync(cancellationToken);

            if (!isAssigned)
            {
                return RequestResult<List<ExaminationAttemptDto>>.Failure(
                    ErrorCode.Unauthorized,
                    "You are not authorized to view attempts for this examination");
            }
        }

        var query = attemptRepository.Get(a =>
            a.ExaminationId == request.ExaminationId && !a.IsDeleted);

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(a => a.Status == request.Status);
        }

        var attempts = await query
            .OrderByDescending(a => a.SubmittedAt ?? a.StartedAt)
            .ToListAsync(cancellationToken);

        var result = attempts.Select(a => new ExaminationAttemptDto
        {
            Id = a.ID,
            ExaminationId = a.ExaminationId,
            StudentId = a.StudentId,
            StudentName = a.Student?.Name ?? "Unknown",
            StudentEmail = a.Student?.Email ?? "",
            StartedAt = a.StartedAt,
            SubmittedAt = a.SubmittedAt,
            Answers = a.Answers,
            Score = a.Score,
            Status = a.Status,
            TimeSpent = a.TimeSpent,
            GradedBy = a.GradedBy,
            GraderName = a.Grader?.Name,
            GradedAt = a.GradedAt
        }).ToList();

        return RequestResult<List<ExaminationAttemptDto>>.Success(result);
    }
}

