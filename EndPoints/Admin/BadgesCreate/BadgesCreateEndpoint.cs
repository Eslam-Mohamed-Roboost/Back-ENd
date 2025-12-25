using API.Application.Features.Admin.Badges.Commands;
using API.Application.Features.Admin.Badges.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgesCreate;

public class BadgesCreateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/Badges",
                async (IMediator mediator, CreateBadgeDto request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateBadgeCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();
    }
}
