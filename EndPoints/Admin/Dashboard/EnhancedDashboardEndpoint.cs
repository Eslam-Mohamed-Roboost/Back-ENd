using API.Application.Features.Admin.Dashboard.DTOs;
using API.Application.Features.Admin.Dashboard.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.Dashboard;

public class EnhancedDashboardEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Dashboard/Enhanced",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetEnhancedDashboardQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<EnhancedDashboardDto>>();
    }
}

