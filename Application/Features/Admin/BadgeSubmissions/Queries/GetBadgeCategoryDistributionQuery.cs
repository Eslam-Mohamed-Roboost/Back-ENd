using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Domain.Entities.Gamification;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.BadgeSubmissions.Queries;

public record GetBadgeCategoryDistributionQuery : IRequest<RequestResult<List<CategoryCountDto>>>;

public class GetBadgeCategoryDistributionQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentBadges> studentBadgesRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<GetBadgeCategoryDistributionQuery, RequestResult<List<CategoryCountDto>>>(parameters)
{
    public override async Task<RequestResult<List<CategoryCountDto>>> Handle(GetBadgeCategoryDistributionQuery request, CancellationToken cancellationToken)
    {
        var byCategory = await (
            from sb in studentBadgesRepository.Get()
            join b in badgesRepository.Get() on sb.BadgeId equals b.ID
            group sb by b.Category into g
            select new CategoryCountDto
            {
                Category = g.Key.ToString(),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<CategoryCountDto>>.Success(byCategory);
    }
}
