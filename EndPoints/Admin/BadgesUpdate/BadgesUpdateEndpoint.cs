using API.Application.Features.Admin.Badges.Commands;
using API.Application.Features.Admin.Badges.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgesUpdate;

public class BadgesUpdateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/Badges/{id}",
                async (IMediator mediator, long id, UpdateBadgeDto request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateBadgeCommand(id, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

