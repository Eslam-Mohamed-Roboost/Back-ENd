using API.Domain.Entities.Academic;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Exercises.Commands;

public record DeleteExerciseCommand(long ExerciseId) : IRequest<RequestResult<bool>>;

public class DeleteExerciseCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Exercises> exerciseRepository)
    : RequestHandlerBase<DeleteExerciseCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        DeleteExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var exercise = await exerciseRepository.Get(e =>
            e.ID == request.ExerciseId && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (exercise == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Exercise not found");
        }

        if (exercise.TeacherId != teacherId)
        {
            return RequestResult<bool>.Failure(
                ErrorCode.Unauthorized,
                "You can only delete your own exercises");
        }

        // Soft delete
        exercise.IsDeleted = true;
        exercise.UpdatedAt = DateTime.UtcNow;
        exercise.UpdatedBy = teacherId;

        exerciseRepository.Update(exercise);
        await exerciseRepository.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.Success(true);
    }
}

