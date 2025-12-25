using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Application.Features.Admin.BadgeSubmissions.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgeSubmissionsGetById;

public class BadgeSubmissionsGetByIdEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/BadgeSubmissions/{id}",
                async (long id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetBadgeSubmissionByIdQuery(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<BadgeSubmissionDto>>();
    }
}

