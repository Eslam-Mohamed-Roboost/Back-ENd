using API.Application.Features.Admin.Portfolio.DTOs;
using API.Application.Features.Admin.Portfolio.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.PortfolioStatsByClass;

public class PortfolioStatsByClassEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Portfolio/StatsByClass",
                async (IMediator mediator, string? className, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetPortfolioStatsByClassQuery(className), cancellationToken);
                    return Response(result);
                 })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<ClassPortfolioStatsDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
