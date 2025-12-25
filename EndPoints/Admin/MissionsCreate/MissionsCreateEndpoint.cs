using API.Application.Features.Admin.Missions.Commands;
using API.Application.Features.Admin.Missions.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.MissionsCreate;

public class MissionsCreateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/Missions",
                async (IMediator mediator, CreateMissionDto request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateMissionCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();
    }
}


