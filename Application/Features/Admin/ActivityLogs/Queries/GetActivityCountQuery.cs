using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Infrastructure.Persistence.Repositories;
using ActivityLogsEntity = API.Domain.Entities.System.ActivityLogs;

namespace API.Application.Features.Admin.ActivityLogs.Queries;

public record GetActivityCountQuery(bool IsToday = true) : IRequest<RequestResult<int>>;

public class GetActivityCountQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ActivityLogsEntity> repository)
    : RequestHandlerBase<GetActivityCountQuery, RequestResult<int>>(parameters)
{
    public override async Task<RequestResult<int>> Handle(GetActivityCountQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Get();
        
        if (request.IsToday)
        {
            var today = DateTime.UtcNow.Date;
            query = query.Where(l => l.CreatedAt.Date == today);
        }
        else
        {
            var weekStart = DateTime.UtcNow.AddDays(-7);
            query = query.Where(l => l.CreatedAt >= weekStart);
        }

        var count = await query.CountAsync(cancellationToken);
        return RequestResult<int>.Success(count);
    }
}
