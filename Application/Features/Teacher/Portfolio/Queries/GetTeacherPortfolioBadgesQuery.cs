using API.Application.Features.Student.Badges.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Portfolio.Queries;

public record GetTeacherPortfolioBadgesQuery : IRequest<RequestResult<List<BadgeDto>>>;

public class GetTeacherPortfolioBadgesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Badges> badgesRepository)
    : RequestHandlerBase<GetTeacherPortfolioBadgesQuery, RequestResult<List<BadgeDto>>>(parameters)
{
    public override async Task<RequestResult<List<BadgeDto>>> Handle(GetTeacherPortfolioBadgesQuery request, CancellationToken cancellationToken)
    {
        var badges = await badgesRepository.Get(x => x.IsActive && x.TargetRole != BadgeTargetRole.Student)
            .OrderBy(x => x.Name)
            .Select(x => new BadgeDto
            {
                Id = x.ID,
                Name = x.Name,
                Icon = x.Icon ?? string.Empty,
                Earned = false,
                EarnDate = null,
                Requirement = x.Description ?? string.Empty,
                Category = x.Category.ToString()
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<BadgeDto>>.Success(badges);
    }
}


