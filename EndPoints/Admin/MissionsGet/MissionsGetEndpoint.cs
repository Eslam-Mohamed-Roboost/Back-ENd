using API.Application.Features.Admin.Missions.DTOs;
using API.Application.Features.Admin.Missions.Queries;
using API.Filters;
using API.Helpers;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.MissionsGet;

public class MissionsGetEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Missions",
                async (IMediator mediator, [AsParameters] GetMissionsRequest request, CancellationToken cancellationToken) =>
                {
                    DefaultValueHelper.ApplyDefaults(request);
                    var result = await mediator.Send(new GetMissionsQuery(request.page, request.pageSize, request.badgeId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PagingDto<MissionDto>>>();
    }
}

