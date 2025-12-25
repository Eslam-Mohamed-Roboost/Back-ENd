using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Domain.Entities.Gamification;
using API.Helpers;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.BadgeSubmissions.Queries;

public record GetBadgeSubmissionsQuery(
    int PageIndex = 1, 
    int PageSize = 50, 
    string? Status = null, 
    int? UserRole = null,
    string? Category = null) 
    : IRequest<RequestResult<PagingDto<BadgeSubmissionDto>>>;

public class GetBadgeSubmissionsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentBadges> repository)
    : RequestHandlerBase<GetBadgeSubmissionsQuery, RequestResult<PagingDto<BadgeSubmissionDto>>>(parameters)
{
    public override async Task<RequestResult<PagingDto<BadgeSubmissionDto>>> Handle(GetBadgeSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Get();
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var submissions = await query
            .OrderByDescending(s => s.EarnedDate)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new BadgeSubmissionDto
            {
                Id = s.ID,
                UserId = s.StudentId,
                BadgeId = s.BadgeId,
                SubmissionDate = s.EarnedDate,
                Status = s.Status.GetDescription()
            })
            .ToListAsync(cancellationToken);

        var result = new PagingDto<BadgeSubmissionDto>(request.PageSize, request.PageIndex, totalCount, totalPages, submissions);
        return RequestResult<PagingDto<BadgeSubmissionDto>>.Success(result);
    }
}
