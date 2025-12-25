using API.Application.Features.Admin.Missions.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MissionsEntity = API.Domain.Entities.Missions.Missions;

namespace API.Application.Features.Admin.Missions.Commands;

public record CreateMissionCommand(CreateMissionDto Dto) : IRequest<RequestResult<long>>;

public class CreateMissionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionsEntity> repository)
    : RequestHandlerBase<CreateMissionCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
    {
        // Determine next Number and Order
        var maxNumber = await repository.Get().MaxAsync(m => (int?)m.Number, cancellationToken) ?? 0;
        var maxOrder = await repository.Get().MaxAsync(m => (int?)m.Order, cancellationToken) ?? 0;

        var mission = new MissionsEntity
        {
            Number = maxNumber + 1,
            Title = request.Dto.Title,
            Description = request.Dto.Description,
            Icon = request.Dto.Icon,
            EstimatedMinutes = request.Dto.EstimatedMinutes,
            BadgeId = request.Dto.BadgeId,
            Order = maxOrder + 1,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow
        };

        var id = repository.Add(mission);
        await repository.SaveChangesAsync();

        return RequestResult<long>.Success(id, "Mission created successfully");
    }
}


