using API.Application.Features.Admin.Evidence.DTOs;
using API.Application.Features.Admin.Evidence.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.EvidenceStats;

public class EvidenceStatsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Evidence/Stats",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetEvidenceStatsQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .Produces<EndPointResponse<EvidenceStatsDto>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
