using API.Application.Features.Admin.Missions.Commands;
using API.Application.Features.Admin.Missions.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.MissionsUpdate;

public class MissionsUpdateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/Missions/{id}",
                async (IMediator mediator, long id, UpdateMissionDto request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateMissionCommand(id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

