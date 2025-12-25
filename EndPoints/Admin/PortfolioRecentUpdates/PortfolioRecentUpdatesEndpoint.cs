using API.Application.Features.Admin.Portfolio.DTOs;
using API.Application.Features.Admin.Portfolio.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.PortfolioRecentUpdates;

public class PortfolioRecentUpdatesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Portfolio/RecentUpdates",
                async (IMediator mediator, int? limit, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetPortfolioRecentUpdatesQuery(limit ?? 10), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<PortfolioUpdateDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
