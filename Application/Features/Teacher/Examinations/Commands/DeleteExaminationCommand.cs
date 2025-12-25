using API.Domain.Entities.Academic;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Examinations.Commands;

public record DeleteExaminationCommand(long ExaminationId) : IRequest<RequestResult<bool>>;

public class DeleteExaminationCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Examinations> examinationRepository)
    : RequestHandlerBase<DeleteExaminationCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        DeleteExaminationCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var examination = await examinationRepository.Get(e =>
            e.ID == request.ExaminationId && !e.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (examination == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Examination not found");
        }

        if (examination.TeacherId != teacherId)
        {
            return RequestResult<bool>.Failure(
                ErrorCode.Unauthorized,
                "You can only delete your own examinations");
        }

        // Soft delete
        examination.IsDeleted = true;
        examination.UpdatedAt = DateTime.UtcNow;
        examination.UpdatedBy = teacherId;

        examinationRepository.Update(examination);
        await examinationRepository.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.Success(true);
    }
}

