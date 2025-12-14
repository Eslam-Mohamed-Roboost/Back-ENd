using API.Application.Features.Admin.GetAdminKPI.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin;

public class GetAdminKPIEndpoint: EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/AdminKpi",
                async ( IMediator _mediator, CancellationToken cancellationToken) =>
                {
                     var result = await _mediator.Send(new GetAdminKpiQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<string>>();
    }
}