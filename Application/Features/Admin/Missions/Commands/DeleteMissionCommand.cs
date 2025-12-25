using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MissionEntity = API.Domain.Entities.Missions.Missions;

namespace API.Application.Features.Admin.Missions.Commands;

public record DeleteMissionCommand(long Id) : IRequest<RequestResult<string>>;

public class DeleteMissionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionEntity> repository)
    : RequestHandlerBase<DeleteMissionCommand, RequestResult<string>>(parameters)
{
    public override async Task<RequestResult<string>> Handle(DeleteMissionCommand request, CancellationToken cancellationToken)
    {
        var mission = await repository.Get(m => m.ID == request.Id).FirstOrDefaultAsync(cancellationToken);

        if (mission == null)
        {
            return RequestResult<string>.Failure(ErrorCode.NotFound, "Mission not found");
        }

        repository.Delete(mission);
        await repository.SaveChangesAsync();

        return RequestResult<string>.Success("Mission deleted successfully");
    }
}

