using API.Application.Features.Admin.Missions.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.MissionsSeed;

public class MissionsSeedEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/Missions/Seed",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SeedMissionsCommand(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            //.//AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<int>>();
    }
}


