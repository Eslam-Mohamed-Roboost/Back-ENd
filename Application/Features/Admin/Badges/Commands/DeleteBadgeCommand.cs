using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BadgesEntity = API.Domain.Entities.Gamification.Badges;

namespace API.Application.Features.Admin.Badges.Commands;

public record DeleteBadgeCommand(long Id) : IRequest<RequestResult<string>>;

public class DeleteBadgeCommandHandler(RequestHandlerBaseParameters parameters, IRepository<BadgesEntity> repository)
    : RequestHandlerBase<DeleteBadgeCommand, RequestResult<string>>(parameters)
{
    public override async Task<RequestResult<string>> Handle(DeleteBadgeCommand request, CancellationToken cancellationToken)
    {
        var badge = await repository.Get(b => b.ID == request.Id).FirstOrDefaultAsync(cancellationToken);

        if (badge == null)
        {
            return RequestResult<string>.Failure(ErrorCode.NotFound, "Badge not found");
        }

        repository.Delete(badge);
        await repository.SaveChangesAsync();

        return RequestResult<string>.Success("Badge deleted successfully");
    }
}

