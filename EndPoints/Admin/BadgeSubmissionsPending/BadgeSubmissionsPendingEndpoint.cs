using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Application.Features.Admin.BadgeSubmissions.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgeSubmissionsPending;

public class BadgeSubmissionsPendingEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/BadgeSubmissions/Pending",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetBadgeSubmissionsQuery(1, 50, "Pending"), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PagingDto<BadgeSubmissionDto>>>();
    }
}
