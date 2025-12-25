using API.Application.Features.Admin.ActivityLogs.DTOs;
using API.Application.Features.Admin.ActivityLogs.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.ActivityLogs;

public class ActivityLogsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/ActivityLogs",
                async (IMediator mediator, int page, int pageSize, long? userId, int? type, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetActivityLogsQuery(page, pageSize, userId, type), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PagingDto<ActivityLogDto>>>();
    }
}
