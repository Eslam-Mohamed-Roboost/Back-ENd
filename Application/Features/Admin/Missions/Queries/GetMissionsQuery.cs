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

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var missions = await query
            .OrderBy(m => m.Order)
            .ThenBy(m => m.Number)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Join(
                badgeRepository.Get(),
                mission => mission.BadgeId,
                badge => badge.ID,
                (mission, badge) => new MissionDto
                {
                    Id = mission.ID,
                    Number = mission.Number,
                    Title = mission.Title,
                    Description = mission.Description,
                    Icon = mission.Icon,
                    EstimatedMinutes = mission.EstimatedMinutes,
                    BadgeId = mission.BadgeId,
                    BadgeName = badge.Name,
                    Order = mission.Order,
                    IsEnabled = mission.IsEnabled,
                    CreatedAt = mission.CreatedAt
                })
            .ToListAsync(cancellationToken);

        var result = new PagingDto<MissionDto>(request.PageSize, request.Page, totalCount, totalPages, missions);

        return RequestResult<PagingDto<MissionDto>>.Success(result);
    }
}

