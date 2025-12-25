using API.Application.Features.Admin.Missions.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MissionsEntity = API.Domain.Entities.Missions.Missions;

namespace API.Application.Features.Admin.Missions.Commands;

public record UpdateMissionCommand(long Id, UpdateMissionDto Dto) : IRequest<RequestResult<bool>>;

public class UpdateMissionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionsEntity> repository)
    : RequestHandlerBase<UpdateMissionCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(UpdateMissionCommand request, CancellationToken cancellationToken)
    {
        var mission = await repository.Get()
            .FirstOrDefaultAsync(m => m.ID == request.Id, cancellationToken);

        if (mission == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Mission not found");
        }

        // Update mission properties
        mission.Title = request.Dto.Title;
        mission.Description = request.Dto.Description;
        mission.Icon = request.Dto.Icon;
        mission.EstimatedMinutes = request.Dto.Duration;
        mission.BadgeId = request.Dto.BadgeId;
        mission.Order = request.Dto.Order;
        mission.IsEnabled = request.Dto.Enabled;
        mission.UpdatedAt = DateTime.UtcNow;

        repository.Update(mission);
        await repository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "Mission updated successfully");
    }
}

