using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WeeklyChallengesEntity = API.Domain.Entities.Teacher.WeeklyChallenges;

namespace API.Application.Features.Admin.WeeklyChallenges.Commands;

public record DeleteWeeklyChallengeCommand(long Id) : IRequest<RequestResult<bool>>;

public class DeleteWeeklyChallengeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<WeeklyChallengesEntity> repository)
    : RequestHandlerBase<DeleteWeeklyChallengeCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(DeleteWeeklyChallengeCommand request, CancellationToken cancellationToken)
    {
        var weeklyChallenge = await repository.Get()
            .FirstOrDefaultAsync(wc => wc.ID == request.Id, cancellationToken);

        if (weeklyChallenge == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Weekly challenge not found");
        }

        repository.Delete(weeklyChallenge);
        await repository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "Weekly challenge deleted successfully");
    }
}

