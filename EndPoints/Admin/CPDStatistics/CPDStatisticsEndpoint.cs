using API.Application.Features.Admin.CPD.DTOs;
using API.Application.Features.Admin.CPD.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.CPDStatistics;

public class CPDStatisticsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/CPD/Statistics",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCPDStatisticsQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .Produces<EndPointResponse<CPDStatisticsDto>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
