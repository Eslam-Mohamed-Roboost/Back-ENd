using API.Domain.Entities.Exercises;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ExercisesEntity = API.Domain.Entities.Exercises.Exercises;
using UserEntity = API.Domain.Entities.User;

namespace API.Application.Features.Student.Exercises.Queries;

public record ExerciseDto(
    long Id,
    string Title,
    string Description,
    string Type,
    long SubjectId,
    DateTime? DueDate,
    int MaxPoints,
    string? Instructions,
    bool IsSubmitted,
    decimal? Grade,
    string Status,
    bool IsLate);

public record GetAssignedExercisesQuery : IRequest<RequestResult<List<ExerciseDto>>>;

public class GetAssignedExercisesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ExercisesEntity> exercisesRepository,
    IRepository<ExerciseSubmissions> submissionsRepository,
    IRepository<UserEntity> userRepository)
    : RequestHandlerBase<GetAssignedExercisesQuery, RequestResult<List<ExerciseDto>>>(parameters)
{
    public override async Task<RequestResult<List<ExerciseDto>>> Handle(
        GetAssignedExercisesQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get student info to find their class
        var student = await userRepository
            .Get(u => u.ID == studentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (student == null || !student.ClassID.HasValue)
        {
            return RequestResult<List<ExerciseDto>>.Success(new List<ExerciseDto>());
        }

        var classId = student.ClassID.Value;

        // Get all exercises for student's class
        var exercises = await exercisesRepository
            .Get(e => e.ClassId == classId && e.IsActive)
            .OrderByDescending(e => e.AssignedAt)
            .ToListAsync(cancellationToken);

        // Get student's submissions
        var submissions = await submissionsRepository
            .Get(s => s.StudentId == studentId)
            .ToListAsync(cancellationToken);

        var exerciseDtos = exercises.Select(e =>
        {
            var submission = submissions.FirstOrDefault(s => s.ExerciseId == e.ID);
            var isSubmitted = submission != null;
            var status = isSubmitted
                ? (submission!.Grade.HasValue ? "Graded" : "Pending Review")
                : (e.DueDate.HasValue && DateTime.UtcNow > e.DueDate.Value ? "Overdue" : "Not Submitted");

            return new ExerciseDto(
                e.ID,
                e.Title,
                e.Description,
                e.Type.ToString(),
                e.SubjectId ?? 0,
                e.DueDate,
                e.MaxPoints,
                e.Instructions,
                isSubmitted,
                submission?.Grade,
                status,
                submission?.IsLate ?? false);
        }).ToList();

        return RequestResult<List<ExerciseDto>>.Success(exerciseDtos);
    }
}

