using API.Application.Features.Admin.Missions.DTOs;
using API.Application.Features.Admin.Missions.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.Missions;

public class MissionProgressEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Admin/Missions/Progress - Get overall mission progress
        app.MapGet("/Admin/Missions/Progress",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetMissionProgressQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<MissionProgressOverviewDto>>();
    }
}

