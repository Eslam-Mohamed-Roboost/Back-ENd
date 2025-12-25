using API.Domain.Entities.Gamification;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.BadgeSubmissions.Queries;

public record BadgeStatusCountsDto(int Total, int Approved, int Rejected, int Pending);

public record GetBadgeSubmissionStatusCountsQuery : IRequest<RequestResult<BadgeStatusCountsDto>>;

public class GetBadgeSubmissionStatusCountsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentBadges> studentBadgesRepository)
    : RequestHandlerBase<GetBadgeSubmissionStatusCountsQuery, RequestResult<BadgeStatusCountsDto>>(parameters)
{
    public override async Task<RequestResult<BadgeStatusCountsDto>> Handle(GetBadgeSubmissionStatusCountsQuery request, CancellationToken cancellationToken)
    {
        var totalEarned = await studentBadgesRepository.Get().CountAsync(cancellationToken);
        
        var statusCounts = await studentBadgesRepository.Get()
            .GroupBy(sb => sb.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var approved = statusCounts.FirstOrDefault(s => s.Status == Status.Approved)?.Count ?? 0;
        var rejected = statusCounts.FirstOrDefault(s => s.Status == Status.Rejected)?.Count ?? 0;
        var pending = statusCounts.FirstOrDefault(s => s.Status == Status.Pinndeing)?.Count ?? 0;

        return RequestResult<BadgeStatusCountsDto>.Success(new BadgeStatusCountsDto(totalEarned, approved, rejected, pending));
    }
}
