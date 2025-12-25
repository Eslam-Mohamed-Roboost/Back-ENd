using API.Application.Features.Admin.ActivityLogs.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ActivityLogsEntity = API.Domain.Entities.System.ActivityLogs;

namespace API.Application.Features.Admin.ActivityLogs.Queries;

public record GetActivityLogsQuery(int Page = 1, int PageSize = 50, long? UserId = null, int? Type = null) 
    : IRequest<RequestResult<PagingDto<ActivityLogDto>>>;

public class GetActivityLogsQueryHandler(
    RequestHandlerBaseParameters parameters, 
    IRepository<ActivityLogsEntity> repository)
    : RequestHandlerBase<GetActivityLogsQuery, RequestResult<PagingDto<ActivityLogDto>>>(parameters)
{
    public override async Task<RequestResult<PagingDto<ActivityLogDto>>> Handle(GetActivityLogsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Get();

        if (request.UserId.HasValue)
        {
            query = query.Where(l => l.UserId == request.UserId.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(l => (int)l.Type == request.Type.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new ActivityLogDto
            {
                Id = l.ID,
                UserId = l.UserId,
                UserName = l.UserName??string.Empty,
                Action = l.Action,
                Type = l.Type.ToString(),
                Details = l.Details,
                IpAddress = l.IpAddress,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var result = new PagingDto<ActivityLogDto>(request.PageSize, request.Page, totalCount, totalPages, logs);

        return RequestResult<PagingDto<ActivityLogDto>>.Success(result);
    }
}
