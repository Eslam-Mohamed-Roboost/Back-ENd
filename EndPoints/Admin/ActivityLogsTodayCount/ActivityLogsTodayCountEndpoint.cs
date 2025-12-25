using API.Application.Features.Admin.ActivityLogs.Queries;
using API.Filters;
using MediatR;

namespace API.EndPoints.Admin.ActivityLogsTodayCount;

public class ActivityLogsTodayCountEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/ActivityLogs/TodayCount",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetActivityCountQuery(true), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
