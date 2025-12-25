using API.Application.Features.Admin.Badges.DTOs;
using API.Application.Features.Admin.Badges.Queries;
using API.Domain.Enums;
using API.EndPoints.Admin.GetBadges;
using API.Filters;
using API.Helpers;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgesGet;

public class GetBadgesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Badges",
                async (IMediator mediator, [AsParameters] GetBadgesRequest request, CancellationToken cancellationToken) =>
                {
                    DefaultValueHelper.ApplyDefaults(request);
                    var result = await mediator.Send(new GetBadgesQuery(request.page, request.pageSize, request.category), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PagingDto<BadgeDto>>>();
    }
}
