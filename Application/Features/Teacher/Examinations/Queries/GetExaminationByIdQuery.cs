using API.Application.Features.Teacher.Examinations.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Examinations.Queries;

public record GetExaminationByIdQuery(long ExaminationId) : IRequest<RequestResult<ExaminationDto>>;

public class GetExaminationByIdQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Examinations> examinationRepository,
    IRepository<ExaminationAttempts> attemptRepository,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetExaminationByIdQuery, RequestResult<ExaminationDto>>(parameters)
{
    public override async Task<RequestResult<ExaminationDto>> Handle(
        GetExaminationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var examination = await examinationRepository.Get(e =>
            e.ID == request.ExaminationId && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (examination == null)
        {
            return RequestResult<ExaminationDto>.Failure(ErrorCode.NotFound, "Examination not found");
        }

        var isAssigned = await assignmentRepository.Get(a =>
            a.TeacherId == teacherId &&
            a.ClassId == examination.ClassId &&
            a.SubjectId == examination.SubjectId &&
            !a.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!isAssigned)
        {
            return RequestResult<ExaminationDto>.Failure(
                ErrorCode.Unauthorized,
                "You are not assigned to this examination's class/subject");
        }

        var attemptCount = await attemptRepository.Get(a =>
            a.ExaminationId == examination.ID && !a.IsDeleted)
            .CountAsync(cancellationToken);

        var gradedCount = await attemptRepository.Get(a =>
            a.ExaminationId == examination.ID &&
            a.Status == "Graded" &&
            !a.IsDeleted)
            .CountAsync(cancellationToken);

        var result = new ExaminationDto
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
        };

        return RequestResult<ExaminationDto>.Success(result);
    }
}

