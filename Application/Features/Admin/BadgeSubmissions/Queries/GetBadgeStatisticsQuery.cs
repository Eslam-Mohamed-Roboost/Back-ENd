using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Admin.BadgeSubmissions.Queries;

public record GetBadgeStatisticsQuery : IRequest<RequestResult<BadgeStatisticsDto>>;

public class GetBadgeStatisticsQueryHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<GetBadgeStatisticsQuery, RequestResult<BadgeStatisticsDto>>(parameters)
{
    public override async Task<RequestResult<BadgeStatisticsDto>> Handle(GetBadgeStatisticsQuery request, CancellationToken cancellationToken)
    {
        // 1. Get Status Counts
        var statusCountsResult = await _mediator.Send(new GetBadgeSubmissionStatusCountsQuery(), cancellationToken);
        var counts = statusCountsResult.Data ?? new BadgeStatusCountsDto(0, 0, 0, 0);

        // 2. Get Category Distribution
        var categoryDistributionResult = await _mediator.Send(new GetBadgeCategoryDistributionQuery(), cancellationToken);
        var byCategory = categoryDistributionResult.Data ?? new List<CategoryCountDto>();

        // 3. Calculate Rates (could also be done in the sub-query, but logic here is fine)
        var approvalRate = counts.Total > 0 ? (int)((counts.Approved * 100.0) / counts.Total) : 0;
        var rejectionRate = counts.Total > 0 ? (int)((counts.Rejected * 100.0) / counts.Total) : 0;

        var result = new BadgeStatisticsDto
        {
            Total = counts.Total,
            Approved = counts.Approved,
            Rejected = counts.Rejected,
            Pending = counts.Pending,
            ApprovalRate = approvalRate,
            RejectionRate = rejectionRate,
            ByCategory = byCategory
        };

        return RequestResult<BadgeStatisticsDto>.Success(result);
    }
}
