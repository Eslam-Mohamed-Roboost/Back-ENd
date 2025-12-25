using API.Application.Features.Admin.Badges.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BadgesEntity = API.Domain.Entities.Gamification.Badges;

namespace API.Application.Features.Admin.Badges.Queries;

public record GetBadgesQuery(int Page = 1, int PageSize = 20, BadgeCategory? Category = null) 
    : IRequest<RequestResult<PagingDto<BadgeDto>>>;

public class GetBadgesQueryHandler(RequestHandlerBaseParameters parameters, IRepository<BadgesEntity> repository)
    : RequestHandlerBase<GetBadgesQuery, RequestResult<PagingDto<BadgeDto>>>(parameters)
{
    public override async Task<RequestResult<PagingDto<BadgeDto>>> Handle(GetBadgesQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Get();

        if (request.Category.HasValue)
        {
            query = query.Where(b => b.Category == request.Category.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var badges = await query
            .OrderBy(b => b.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BadgeDto
            {
                Id = b.ID,
                Name = b.Name,
                Description = b.Description,
                Icon = b.Icon,
                Color = b.Color,
                Category = (int)b.Category,
                CategoryName = b.Category.ToString(),
                TargetRole = (int)b.TargetRole,
                TargetRoleName = b.TargetRole.ToString(),
                CpdHours = b.CpdHours,
                IsActive = b.IsActive,
                EarnedCount = 0
            })
            .ToListAsync(cancellationToken);

        var result = new PagingDto<BadgeDto>(request.PageSize, request.Page, totalCount, totalPages, badges);

        return RequestResult<PagingDto<BadgeDto>>.Success(result);
    }
}
