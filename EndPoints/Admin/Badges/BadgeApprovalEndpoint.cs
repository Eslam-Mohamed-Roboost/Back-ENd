using API.Application.Features.Admin.Badges.Commands;
using API.Application.Features.Admin.Badges.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.Badges;

public class BadgeApprovalEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Admin/Badges/Pending - Get pending badge approvals
        app.MapGet("/Admin/Badges/Pending",
                async (long? classId, long? studentId, long? badgeId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetPendingBadgesQuery(classId, studentId, badgeId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<PendingBadgeDto>>>();

        // POST /Admin/Badges/Approve/{id} - Approve badge
        app.MapPost("/Admin/Badges/Approve/{id}",
                async (long id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new ApproveBadgeCommand(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

