using API.Application.Features.Admin.Portfolio.DTOs;
using API.Application.Features.Admin.Portfolio.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.PortfolioCompletionStats;

public class PortfolioCompletionStatsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Portfolio/CompletionStats",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetPortfolioCompletionStatsQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioCompletionStatsDto>>();
    }
}
