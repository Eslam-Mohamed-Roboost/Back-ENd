using API.Application.Features.Teacher.Examinations.DTOs;
using API.Application.Features.Teacher.Permissions.Services;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Teacher.Examinations.Commands;

public record CreateExaminationCommand(CreateExaminationRequest Request) : IRequest<RequestResult<ExaminationDto>>;

public class CreateExaminationCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Examinations> examinationRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<TeacherClassAssignments> assignmentRepository,
    TeacherPermissionService permissionService)
    : RequestHandlerBase<CreateExaminationCommand, RequestResult<ExaminationDto>>(parameters)
{
    public override async Task<RequestResult<ExaminationDto>> Handle(
        CreateExaminationCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Check permission
        var canCreate = await permissionService.CanCreateExaminationsAsync(
            teacherId,
            request.Request.ClassId,
            request.Request.SubjectId,
            cancellationToken);

        if (!canCreate)
        {
            return RequestResult<ExaminationDto>.Failure(
                ErrorCode.Unauthorized,
                "You do not have permission to create examinations for this class/subject");
        }

        // Validate class exists
        var classExists = await classRepository.Get(c =>
            c.ID == request.Request.ClassId && !c.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!classExists)
        {
            return RequestResult<ExaminationDto>.Failure(
                ErrorCode.NotFound,
                "Class not found");
        }

        // Create examination
        var examination = new Examinations
        {
            TeacherId = teacherId,
            ClassId = request.Request.ClassId,
            SubjectId = request.Request.SubjectId,
            Title = request.Request.Title,
            Description = request.Request.Description,
            Type = request.Request.Type,
            ScheduledDate = request.Request.ScheduledDate,
            Duration = request.Request.Duration,
            MaxScore = request.Request.MaxScore,
            Instructions = request.Request.Instructions,
            Questions = request.Request.Questions,
            Status = request.Request.Status,
            CreatedAt = DateTime.UtcNow
        };

        examinationRepository.Add(examination);
        await examinationRepository.SaveChangesAsync(cancellationToken);

        // Return DTO
        var result = new ExaminationDto
        {
            Id = examination.ID,
            TeacherId = examination.TeacherId,
            TeacherName = "Current Teacher",
            ClassId = examination.ClassId,
            ClassName = "",
            SubjectId = examination.SubjectId,
            SubjectName = "",
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

