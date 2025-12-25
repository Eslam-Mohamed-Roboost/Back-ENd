using API.Application.Features.Admin.ActivityLogs.Queries;
using API.Filters;
using MediatR;

namespace API.EndPoints.Admin.ActivityLogsWeekCount;

public class ActivityLogsWeekCountEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/ActivityLogs/WeekCount",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetActivityCountQuery(false), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
