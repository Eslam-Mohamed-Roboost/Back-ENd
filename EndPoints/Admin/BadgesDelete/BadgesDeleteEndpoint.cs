using API.Application.Features.Admin.Badges.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgesDelete;

public class BadgesDeleteEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Admin/Badges/{id}",
                async (long id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new DeleteBadgeCommand(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<string>>();
    }
}

