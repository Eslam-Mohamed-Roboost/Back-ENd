using API.Application.Features.Teacher.Examinations.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Examinations.Commands;

public record UpdateExaminationCommand(long ExaminationId, UpdateExaminationRequest Request) : IRequest<RequestResult<ExaminationDto>>;

public class UpdateExaminationCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Examinations> examinationRepository)
    : RequestHandlerBase<UpdateExaminationCommand, RequestResult<ExaminationDto>>(parameters)
{
    public override async Task<RequestResult<ExaminationDto>> Handle(
        UpdateExaminationCommand request,
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

        if (examination.TeacherId != teacherId)
        {
            return RequestResult<ExaminationDto>.Failure(
                ErrorCode.Unauthorized,
                "You can only update your own examinations");
        }

        // Update fields
        if (!string.IsNullOrEmpty(request.Request.Title))
            examination.Title = request.Request.Title;

        if (request.Request.Description != null)
            examination.Description = request.Request.Description;

        if (!string.IsNullOrEmpty(request.Request.Type))
            examination.Type = request.Request.Type;

        if (request.Request.ScheduledDate.HasValue)
            examination.ScheduledDate = request.Request.ScheduledDate;

        if (request.Request.Duration.HasValue)
            examination.Duration = request.Request.Duration;

        if (request.Request.MaxScore.HasValue)
            examination.MaxScore = request.Request.MaxScore.Value;

        if (request.Request.Instructions != null)
            examination.Instructions = request.Request.Instructions;

        if (request.Request.Questions != null)
            examination.Questions = request.Request.Questions;

        if (!string.IsNullOrEmpty(request.Request.Status))
            examination.Status = request.Request.Status;

        examination.UpdatedAt = DateTime.UtcNow;
        examination.UpdatedBy = teacherId;

        examinationRepository.Update(examination);
        await examinationRepository.SaveChangesAsync();

        // Return DTO
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
            AttemptCount = 0,
            GradedCount = 0
        };

        return RequestResult<ExaminationDto>.Success(result);
    }
}

