using API.Application.Features.Teacher.Exercises.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Teacher.Exercises.Queries;

public record GetExercisesQuery(
    long? ClassId = null,
    long? SubjectId = null,
    string? Type = null,
    string? Status = null) : IRequest<RequestResult<List<ExerciseDto>>>;

public class GetExercisesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Exercises> exerciseRepository,
    IRepository<ExerciseSubmissions> submissionRepository,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetExercisesQuery, RequestResult<List<ExerciseDto>>>(parameters)
{
    public override async Task<RequestResult<List<ExerciseDto>>> Handle(
        GetExercisesQuery request,
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
            return RequestResult<List<ExerciseDto>>.Success(new List<ExerciseDto>());
        }

        // Build query
        var query = exerciseRepository.Get(e =>
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

        var exercises = await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);

        var result = new List<ExerciseDto>();

        foreach (var exercise in exercises)
        {
            var submissionCount = await submissionRepository.Get(s =>
                s.ExerciseId == exercise.ID && !s.IsDeleted)
                .CountAsync(cancellationToken);

            var gradedCount = await submissionRepository.Get(s =>
                s.ExerciseId == exercise.ID &&
                s.Status == "Graded" &&
                !s.IsDeleted)
                .CountAsync(cancellationToken);

            result.Add(new ExerciseDto
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
            });
        }

        return RequestResult<List<ExerciseDto>>.Success(result);
    }
}

