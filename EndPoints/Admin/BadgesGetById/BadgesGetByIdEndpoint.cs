using API.Application.Features.Admin.Badges.DTOs;
using API.Application.Features.Admin.Badges.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgesGetById;

public class BadgesGetByIdEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Badges/{id}",
                async (long id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetBadgeByIdQuery(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<BadgeDto>>();
    }
}

