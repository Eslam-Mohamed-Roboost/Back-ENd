using API.Application.Features.Admin.Missions.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MissionEntity = API.Domain.Entities.Missions.Missions;

namespace API.Application.Features.Admin.Missions.Queries;

public record GetMissionByIdQuery(long Id) : IRequest<RequestResult<MissionDto>>;

public class GetMissionByIdQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionEntity> repository)
    : RequestHandlerBase<GetMissionByIdQuery, RequestResult<MissionDto>>(parameters)
{
    public override async Task<RequestResult<MissionDto>> Handle(GetMissionByIdQuery request, CancellationToken cancellationToken)
    {
        var mission = await repository.Get(m => m.ID == request.Id)
            .Select(m => new MissionDto
            {
                Id = m.ID,
                Number = m.Number,
                Title = m.Title,
                Description = m.Description,
                Icon = m.Icon,
                EstimatedMinutes = m.EstimatedMinutes,
                BadgeId = m.BadgeId,
                BadgeName = null, // Badge navigation property not included in entity
                Order = m.Order,
                IsEnabled = m.IsEnabled,
                CreatedAt = m.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (mission == null)
        {
            return RequestResult<MissionDto>.Failure(ErrorCode.NotFound, "Mission not found");
        }

        return RequestResult<MissionDto>.Success(mission);
    }
}

