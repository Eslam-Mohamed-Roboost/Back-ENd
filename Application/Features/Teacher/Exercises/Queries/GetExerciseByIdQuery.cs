using API.Application.Features.Teacher.Exercises.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Exercises.Queries;

public record GetExerciseByIdQuery(long ExerciseId) : IRequest<RequestResult<ExerciseDto>>;

public class GetExerciseByIdQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Exercises> exerciseRepository,
    IRepository<ExerciseSubmissions> submissionRepository,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetExerciseByIdQuery, RequestResult<ExerciseDto>>(parameters)
{
    public override async Task<RequestResult<ExerciseDto>> Handle(
        GetExerciseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Verify teacher is assigned to this exercise's class
        var exercise = await exerciseRepository.Get(e =>
            e.ID == request.ExerciseId && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (exercise == null)
        {
            return RequestResult<ExerciseDto>.Failure(ErrorCode.NotFound, "Exercise not found");
        }

        var isAssigned = await assignmentRepository.Get(a =>
            a.TeacherId == teacherId &&
            a.ClassId == exercise.ClassId &&
            a.SubjectId == exercise.SubjectId &&
            !a.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!isAssigned)
        {
            return RequestResult<ExerciseDto>.Failure(
                ErrorCode.Unauthorized,
                "You are not assigned to this exercise's class/subject");
        }

        var submissionCount = await submissionRepository.Get(s =>
            s.ExerciseId == exercise.ID && !s.IsDeleted)
            .CountAsync(cancellationToken);

        var gradedCount = await submissionRepository.Get(s =>
            s.ExerciseId == exercise.ID &&
            s.Status == "Graded" &&
            !s.IsDeleted)
            .CountAsync(cancellationToken);

        var result = new ExerciseDto
        {
            Id = exercise.ID,
            TeacherId = exercise.TeacherId,
            TeacherName = exercise.Teacher?.Name ?? "Unknown",
            ClassId = exercise.ClassId,
            ClassName = exercise.Class?.Name ?? "Unknown",
            SubjectId = exercise.SubjectId,
            SubjectName = exercise.Subject?.Name ?? "Unknown",
            Title = exercise.Title,
            Description = exercise.Description,
            Type = exercise.Type,
            DueDate = exercise.DueDate,
            MaxScore = exercise.MaxScore,
            Instructions = exercise.Instructions,
            Attachments = exercise.Attachments,
            Status = exercise.Status,
            CreatedAt = exercise.CreatedAt,
            UpdatedAt = exercise.UpdatedAt,
            SubmissionCount = submissionCount,
            GradedCount = gradedCount
        };

        return RequestResult<ExerciseDto>.Success(result);
    }
}

