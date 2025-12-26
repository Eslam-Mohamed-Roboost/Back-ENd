using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Missions.Commands;

public record DeleteMissionResourceCommand(long Id) : IRequest<RequestResult<bool>>;

public class DeleteMissionResourceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionResources> repository)
    : RequestHandlerBase<DeleteMissionResourceCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(DeleteMissionResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.Get(x => x.ID == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (resource == null)
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Resource not found");

        repository.Delete(resource);
        await repository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "Resource deleted successfully");
    }
}

