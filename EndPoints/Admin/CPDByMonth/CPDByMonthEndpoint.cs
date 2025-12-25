using API.Application.Features.Admin.CPD.DTOs;
using API.Application.Features.Admin.CPD.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.CPDByMonth;

public class CPDByMonthEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/CPD/ByMonth",
                async (IMediator mediator, [AsParameters] int monthsBack , CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCPDByMonthQuery(monthsBack), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<CPDByMonthDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
