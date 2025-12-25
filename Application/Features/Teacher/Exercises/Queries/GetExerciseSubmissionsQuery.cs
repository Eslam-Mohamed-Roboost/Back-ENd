using API.Domain.Entities.Exercises;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ExercisesEntity = API.Domain.Entities.Exercises.Exercises;

namespace API.Application.Features.Teacher.Exercises.Queries;

public record SubmissionDto(
    long Id,
    long ExerciseId,
    long StudentId,
    string StudentName,
    string? SubmissionText,
    string? FileUrl,
    DateTime SubmittedAt,
    bool IsLate,
    string Status,
    decimal? Grade,
    string? Feedback,
    DateTime? GradedAt);

public record GetExerciseSubmissionsQuery(long ExerciseId) : IRequest<RequestResult<List<SubmissionDto>>>;

public class GetExerciseSubmissionsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ExerciseSubmissions> submissionsRepository,
    IRepository<ExercisesEntity> exercisesRepository)
    : RequestHandlerBase<GetExerciseSubmissionsQuery, RequestResult<List<SubmissionDto>>>(parameters)
{
    public override async Task<RequestResult<List<SubmissionDto>>> Handle(
        GetExerciseSubmissionsQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Verify teacher owns this exercise
        var exercise = await exercisesRepository
            .Get(e => e.ID == request.ExerciseId && e.TeacherId == teacherId)
            .FirstOrDefaultAsync(cancellationToken);

        if (exercise == null)
        {
            return RequestResult<List<SubmissionDto>>.Failure(ErrorCode.NotFound, "Exercise not found");
        }

        // Get submissions with student info
        var submissions = await submissionsRepository
            .Get(s => s.ExerciseId == request.ExerciseId)
            .Include(s => s.Student)
            .OrderBy(s => s.SubmittedAt)
            .ToListAsync(cancellationToken);

        var submissionDtos = submissions.Select(s => new SubmissionDto(
            s.ID,
            s.ExerciseId,
            s.StudentId,
            s.Student.Name,
            s.SubmissionText,
            s.FileUrl,
            s.SubmittedAt,
            s.IsLate,
            s.Status.ToString(),
            s.Grade,
            s.Feedback,
            s.GradedAt
        )).ToList();

        return RequestResult<List<SubmissionDto>>.Success(submissionDtos);
    }
}

