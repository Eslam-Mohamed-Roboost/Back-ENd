using API.Application.Features.Admin.Badges.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BadgesEntity = API.Domain.Entities.Gamification.Badges;

namespace API.Application.Features.Admin.Badges.Queries;

public record GetBadgeByIdQuery(long Id) : IRequest<RequestResult<BadgeDto>>;

public class GetBadgeByIdQueryHandler(RequestHandlerBaseParameters parameters, IRepository<BadgesEntity> repository)
    : RequestHandlerBase<GetBadgeByIdQuery, RequestResult<BadgeDto>>(parameters)
{
    public override async Task<RequestResult<BadgeDto>> Handle(GetBadgeByIdQuery request, CancellationToken cancellationToken)
    {
        var badge = await repository.Get(b => b.ID == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (badge == null)
        {
            return RequestResult<BadgeDto>.Failure(ErrorCode.NotFound, "Badge not found");
        }

        return RequestResult<BadgeDto>.Success(badge);
    }
}

