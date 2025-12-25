using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Application.Features.Admin.BadgeSubmissions.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgeStatistics;

public class BadgeStatisticsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/BadgeStatistics",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetBadgeStatisticsQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<BadgeStatisticsDto>>();
    }
}
