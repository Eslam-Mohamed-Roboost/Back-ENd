using API.Application.Features.Admin.Settings.DTOs;
using API.Application.Features.Admin.Settings.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.SystemLogs;

public class SystemLogsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/SystemLogs",
                async (IMediator mediator, string? level, DateTime? dateFrom, DateTime? dateTo, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default) =>
                {
                    var result = await mediator.Send(new GetSystemLogsQuery(level, dateFrom, dateTo, pageIndex, pageSize), cancellationToken);
                    return Response(result);
                    
                })
            .WithTags("Admin")
            .Produces<EndPointResponse<PagingDto<SystemLogDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
