using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Application.Features.Admin.BadgeSubmissions.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgeSubmissions;

public record BadgeSubmissionsRequest(
    int page = 1,
    int pageSize = 50,
    string? status = null,
    int? userRole = null,
    string? category = null);

public class BadgeSubmissionsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/BadgeSubmissions",
                async (IMediator mediator, [AsParameters] BadgeSubmissionsRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetBadgeSubmissionsQuery(request.page, request.pageSize, request.status, request.userRole, request.category), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PagingDto<BadgeSubmissionDto>>>();
    }
}
