using API.Application.Features.Teacher.Exercises.DTOs;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Exercises.Queries;

public record GetExerciseSubmissionsQuery(
    long ExerciseId,
    string? Status = null) : IRequest<RequestResult<List<ExerciseSubmissionDto>>>;

public class GetExerciseSubmissionsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.Exercises> exerciseRepository,
    IRepository<ExerciseSubmissions> submissionRepository,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetExerciseSubmissionsQuery, RequestResult<List<ExerciseSubmissionDto>>>(parameters)
{
    public override async Task<RequestResult<List<ExerciseSubmissionDto>>> Handle(
        GetExerciseSubmissionsQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Verify teacher owns this exercise
        var exercise = await exerciseRepository.Get(e =>
            e.ID == request.ExerciseId && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (exercise == null)
        {
            return RequestResult<List<ExerciseSubmissionDto>>.Failure(
                ErrorCode.NotFound,
                "Exercise not found");
        }

        if (exercise.TeacherId != teacherId)
        {
            // Check if teacher is assigned to this class/subject
            var isAssigned = await assignmentRepository.Get(a =>
                a.TeacherId == teacherId &&
                a.ClassId == exercise.ClassId &&
                a.SubjectId == exercise.SubjectId &&
                !a.IsDeleted)
                .AnyAsync(cancellationToken);

            if (!isAssigned)
            {
                return RequestResult<List<ExerciseSubmissionDto>>.Failure(
                    ErrorCode.Unauthorized,
                    "You are not authorized to view submissions for this exercise");
            }
        }

        // Get submissions
        var query = submissionRepository.Get(s =>
            s.ExerciseId == request.ExerciseId && !s.IsDeleted);

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(s => s.Status == request.Status);
        }

        var submissions = await query
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync(cancellationToken);

        var result = submissions.Select(s => new ExerciseSubmissionDto
        {
            Id = s.ID,
            ExerciseId = s.ExerciseId,
            StudentId = s.StudentId,
            StudentName = s.Student?.Name ?? "Unknown",
            StudentEmail = s.Student?.Email ?? "",
            SubmittedAt = s.SubmittedAt,
            Content = s.Content,
            Attachments = s.Attachments,
            Status = s.Status,
            Score = s.Score,
            Feedback = s.Feedback,
            GradedBy = s.GradedBy,
            GraderName = s.Grader?.Name,
            GradedAt = s.GradedAt,
            IsLate = exercise.DueDate.HasValue &&
                     s.SubmittedAt.HasValue &&
                     s.SubmittedAt.Value > exercise.DueDate.Value
        }).ToList();

        return RequestResult<List<ExerciseSubmissionDto>>.Success(result);
    }
}

