using API.Application.Features.Admin.Missions.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MissionsEntity = API.Domain.Entities.Missions.Missions;
using BadgesEntity = API.Domain.Entities.Gamification.Badges;

namespace API.Application.Features.Admin.Missions.Queries;

public record GetMissionsQuery(int Page = 1, int PageSize = 20, long? BadgeId = null) 
    : IRequest<RequestResult<PagingDto<MissionDto>>>;

public class GetMissionsQueryHandler(RequestHandlerBaseParameters parameters, IRepository<MissionsEntity> repository, IRepository<BadgesEntity> badgeRepository)
    : RequestHandlerBase<GetMissionsQuery, RequestResult<PagingDto<MissionDto>>>(parameters)
{
    public override async Task<RequestResult<PagingDto<MissionDto>>> Handle(GetMissionsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Get();

        if (request.BadgeId.HasValue)
        {
            query = query.Where(m => m.BadgeId == request.BadgeId.Value);
        }



        // Get all badges for lookup (to avoid excluding missions without badges)
        var badgeMap = await badgeRepository.Get()
            .ToDictionaryAsync(b => b.ID, b => b.Name, cancellationToken);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pagedMissions = await query
            .OrderBy(m => m.Order)
            .ThenBy(m => m.Number)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs with badge lookup
        var missionDtos = pagedMissions.Select(mission => new MissionDto
        {
            Id = mission.ID,
            Number = mission.Number,
            Title = mission.Title,
            Description = mission.Description,
            Icon = mission.Icon,
            EstimatedMinutes = mission.EstimatedMinutes,
            BadgeId = mission.BadgeId,
            BadgeName = badgeMap.GetValueOrDefault(mission.BadgeId),
            Order = mission.Order,
            IsEnabled = mission.IsEnabled,
            CreatedAt = mission.CreatedAt
        }).ToList();

        var missions = new PagingDto<MissionDto>(
            PageSize: request.PageSize,
            PageIndex: request.Page,
            Records: totalCount,
            Pages: (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Items: missionDtos
        );

 
        return RequestResult<PagingDto<MissionDto>>.Success(missions);
    }
}

